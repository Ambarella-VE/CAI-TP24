namespace Products.API.DTOs;

public record ProductResponse(Guid Id, string Name, string? Description, decimal Price, int Stock, string Category, DateTime CreatedAt);
