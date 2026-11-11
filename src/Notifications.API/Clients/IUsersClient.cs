// [TP Microservicios E-Commerce v7] Define la comunicación de Notifications.API con otro microservicio sin compartir modelos internos.
namespace Notifications.API.Clients;

public interface IUsersClient
{
    bool Exists(Guid userId);
}

public class UsersHttpClient(HttpClient httpClient) : IUsersClient
{
    public bool Exists(Guid userId)
    {
        try
        {
            using var response = httpClient.GetAsync($"api/users/{userId}").GetAwaiter().GetResult();
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException)
        {
            return false;
        }
    }
}
