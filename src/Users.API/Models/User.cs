namespace Users.API.Models;

public enum BlockReason { None, FailedAttempts, Fraud }

public class User
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime RegisteredAt { get; init; }
    public bool Active { get; set; } = true;
    public int FailedAttempts { get; set; }
    public BlockReason BlockReason { get; set; }
}
