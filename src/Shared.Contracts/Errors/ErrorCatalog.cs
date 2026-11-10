namespace Shared.Contracts.Errors;

public record ErrorDefinition(string Code, int StatusCode, string Message);

public static class ErrorCatalog
{
    private static readonly Dictionary<string, ErrorDefinition> Errors = new()
    {
        ["PRD-001"] = new("PRD-001", 404, "Producto no encontrado.")
    };

    public static ErrorDefinition Find(string code)
    {
        return Errors[code];
    }
}
