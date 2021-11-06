using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

public class KeycloakService : IKeycloakService
{
    private readonly HttpClient client;
    private readonly KeycloakConfig keycloakConfig;

    public KeycloakService(HttpClient client, KeycloakConfig keycloakConfig)
    {
        this.client = client;
        this.keycloakConfig = keycloakConfig;
    }

    public async Task CanCreateUser(string userEmail)
    {
        var token = await GetToken();
        var uri = $"auth/admin/realms/{keycloakConfig.Realm}/users?email={userEmail}";
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            uri);
        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Bearer", token);
        var response = await client.SendAsync(request);
        await CheckResponse(response);
        var result = await response.Content.ReadAsStringAsync();
        var users = JsonSerializer.Deserialize<IEnumerable<KCUserDto>>(result);
        if (users?.Any() == true)
        {
            throw new ApplicationException("User already exists");
        }
    }

    public async Task<User> CreateUser(IUserDto createPupilDto)
    {
        var token = await GetToken();
        var uri = $"auth/admin/realms/{keycloakConfig.Realm}/users";
        var payload = new
        {
            username = createPupilDto.Email,
            emailVerified = false,
            enabled = true,
            firstName = createPupilDto.FirstName,
            lastName = createPupilDto.LastName,
            email = createPupilDto.Email
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(
            HttpMethod.Post,
            uri);
        request.Content = content;
        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Bearer", token);
        var response = await client.SendAsync(request);
        await CheckResponse(response);

        var location = response.Headers.Location;

        if (location == null
            || !Guid.TryParse(location.Segments.Last(), out var userKeycloakId))
        {
            throw new ApplicationException("Can't get user id");
        }

        await SendVerificationEmail(userKeycloakId);

        var user = new User(userKeycloakId)
        {
            Email = createPupilDto.Email
        };
        return user;
    }

    public async Task Delete(User user)
    {
        var token = await GetToken();
        var uri = $"auth/admin/realms/{keycloakConfig.Realm}/users/{user.Id}";
        var request = new HttpRequestMessage(
            HttpMethod.Delete,
            uri);
        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Bearer", token);
        var response = await client.SendAsync(request);
        await CheckResponse(response);
    }

    public async Task<string> GetToken()
    {
        var uri = $"auth/realms/{keycloakConfig.Realm}/protocol/openid-connect/token";
        var request = new HttpRequestMessage(
               HttpMethod.Post,
               uri);
        request.Content = new StringContent(
            $"client_secret={keycloakConfig.KeycloakSecret}&client_id={keycloakConfig.KeycloakClient}&grant_type=client_credentials",
            Encoding.UTF8,
            "application/x-www-form-urlencoded");
        var response = await client.SendAsync(request);
        await CheckResponse(response);

        var json = await response.Content.ReadAsStringAsync();
        var token = JObject.Parse(json);

        return (string)token["access_token"];
    }

    public async Task AddUserToGroup(Guid userId, string groupName)
    {
        var groupId = await FindGroupIdByName(groupName);
        var token = await GetToken();
        var uri = $"auth/admin/realms/{keycloakConfig.Realm}/users/{userId}/groups/{groupId}";
        var request = new HttpRequestMessage(HttpMethod.Put, uri);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await client.SendAsync(request);
        await CheckResponse(response);
    }

    private async Task CheckResponse(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var content = await response.Content.ReadAsStringAsync();
        throw new ApplicationException(content);
    }


    private async Task SendVerificationEmail(Guid userId)
    {
        var redirectLink = keycloakConfig.RedirectUrl!.Replace("USER_ID", userId.ToString());
        var lifespan = 48 * 60 * 60;
        var token = await GetToken();
        var uri = $"auth/admin/realms/{keycloakConfig.Realm}/users/{userId}/execute-actions-email?redirect_uri={redirectLink}&client_id={keycloakConfig.KeycloakClient}&lifespan={lifespan}";
        var payload = new[] { "VERIFY_EMAIL", "UPDATE_PASSWORD" };
        var json = JsonSerializer.Serialize(payload);

        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(
               HttpMethod.Put,
               uri);
        request.Content = content;
        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Bearer", token);
        var response = await client.SendAsync(request);
        await CheckResponse(response);
    }

    private async Task<Guid> FindGroupIdByName(string groupName)
    {
        var token = await GetToken();
        var uri = $"auth/admin/realms/{keycloakConfig.Realm}/groups?search={groupName}";
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await client.SendAsync(request);
        await CheckResponse(response);
        var content = await response.Content.ReadAsStringAsync();
        var groups = JsonSerializer.Deserialize<IEnumerable<KCUserGroupDto>>(content)
            ?? throw new NullReferenceException("Failed to get groups");
        return groups.First(group => group.Name == groupName).Id;
    }
}