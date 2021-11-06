using System;
using System.Threading.Tasks;

public interface IKeycloakService
{
    Task CanCreateUser(string userEmail);

    Task<User> CreateUser(IUserDto createPupilDto);

    Task Delete(User user);

    Task<string> GetToken();

    Task AddUserToGroup(Guid userId, string groupName);
}
