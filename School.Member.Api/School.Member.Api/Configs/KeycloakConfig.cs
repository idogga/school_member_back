public class  KeycloakConfig
{
    public string Realm { get; set; } = null!;
    public string KeycloakSecret { get; set; } = null!;
    public string KeycloakClient { get; set; } = null!;
    public string RedirectUrl { get; set; } = null!;
    public string BaseUrl { get; set; } = null!;
    public string Authority { get; set; } = null!;
    public string Audience { get; set; } = null!;
}