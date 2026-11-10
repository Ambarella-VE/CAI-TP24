namespace Orders.API.Clients;

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
