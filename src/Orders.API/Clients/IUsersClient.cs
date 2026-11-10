namespace Orders.API.Clients;

public interface IUsersClient
{
    bool Exists(Guid userId);
}

public class AvailableUsersClient : IUsersClient
{
    public bool Exists(Guid userId) => userId != Guid.Empty;
}
