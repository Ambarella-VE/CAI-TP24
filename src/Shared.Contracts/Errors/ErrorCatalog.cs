// [TP Microservicios E-Commerce v7] Centraliza contratos técnicos reutilizables para responder de forma consistente a la consigna.
namespace Shared.Contracts.Errors;

public enum ErrorCategory
{
    NotFound,
    Validation,
    BusinessRule,
    Unauthorized,
    Forbidden,
    Internal
}

public record ErrorDefinition(string Code, int StatusCode, ErrorCategory Category, string Message);

public static class ErrorCatalog
{
    private static readonly Dictionary<string, ErrorDefinition> Errors = new()
    {
        ["PRD-001"] = new("PRD-001", 404, ErrorCategory.NotFound, "Producto no encontrado."),
        ["PRD-002"] = new("PRD-002", 400, ErrorCategory.Validation, "Los datos del producto son inválidos."),
        ["PRD-003"] = new("PRD-003", 409, ErrorCategory.BusinessRule, "Ya existe un producto con ese nombre en la categoría."),
        ["PRD-004"] = new("PRD-004", 409, ErrorCategory.BusinessRule, "El producto tiene órdenes activas y no puede eliminarse."),
        ["PRD-005"] = new("PRD-005", 500, ErrorCategory.Internal, "Error interno al procesar el producto."),
        ["USR-001"] = new("USR-001", 409, ErrorCategory.BusinessRule, "El email ya está registrado."),
        ["USR-002"] = new("USR-002", 400, ErrorCategory.Validation, "Los datos del usuario son inválidos."),
        ["USR-003"] = new("USR-003", 401, ErrorCategory.Unauthorized, "Credenciales incorrectas."),
        ["USR-004"] = new("USR-004", 403, ErrorCategory.Forbidden, "Usuario bloqueado por demasiados intentos fallidos."),
        ["USR-005"] = new("USR-005", 403, ErrorCategory.Forbidden, "Usuario bloqueado por detección de fraude."),
        ["USR-006"] = new("USR-006", 500, ErrorCategory.Internal, "Error interno al procesar el usuario."),
        ["ORD-001"] = new("ORD-001", 404, ErrorCategory.NotFound, "Orden no encontrada."),
        ["ORD-002"] = new("ORD-002", 400, ErrorCategory.Validation, "Los datos de la orden son inválidos."),
        ["ORD-003"] = new("ORD-003", 404, ErrorCategory.NotFound, "Usuario no encontrado al crear la orden."),
        ["ORD-004"] = new("ORD-004", 404, ErrorCategory.NotFound, "Producto no encontrado al crear la orden."),
        ["ORD-005"] = new("ORD-005", 422, ErrorCategory.BusinessRule, "Stock insuficiente para uno o más productos."),
        ["ORD-006"] = new("ORD-006", 409, ErrorCategory.BusinessRule, "El estado de la orden no puede ser modificado."),
        ["ORD-007"] = new("ORD-007", 500, ErrorCategory.Internal, "Error interno al procesar la orden."),
        ["CRT-001"] = new("CRT-001", 404, ErrorCategory.NotFound, "Carrito no encontrado."),
        ["CRT-002"] = new("CRT-002", 404, ErrorCategory.NotFound, "Producto no encontrado."),
        ["CRT-003"] = new("CRT-003", 422, ErrorCategory.BusinessRule, "Stock insuficiente para agregar al carrito."),
        ["CRT-004"] = new("CRT-004", 400, ErrorCategory.Validation, "Cantidad inválida."),
        ["CRT-005"] = new("CRT-005", 500, ErrorCategory.Internal, "Error interno al procesar el carrito."),
        ["NTF-001"] = new("NTF-001", 404, ErrorCategory.NotFound, "Usuario no encontrado."),
        ["NTF-002"] = new("NTF-002", 400, ErrorCategory.Validation, "Los datos de la notificación son inválidos."),
        ["NTF-003"] = new("NTF-003", 404, ErrorCategory.NotFound, "No se encontraron notificaciones para el usuario."),
        ["NTF-004"] = new("NTF-004", 500, ErrorCategory.Internal, "Error interno al procesar la notificación.")
    };

    public static ErrorDefinition Find(string code)
    {
        return Errors[code];
    }
}
