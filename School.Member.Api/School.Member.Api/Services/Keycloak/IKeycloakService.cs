using System.Threading.Tasks;

public interface IKeycloakService
{
    Task CanCreateUser(CreatePupilDto createPupilDto);
    
    Task<User> CreateUser(CreatePupilDto createPupilDto);

    Task Delete(User user);

    Task<string> GetToken();
}
