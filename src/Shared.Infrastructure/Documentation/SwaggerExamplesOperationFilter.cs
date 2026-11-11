// [TP Microservicios E-Commerce v7] Agrega un ejemplo visible del contrato de error a cada operación documentada en Swagger.
using System.Text.Json.Nodes;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Shared.Infrastructure.Documentation;

/// <summary>Completa las respuestas de error con un ejemplo uniforme que incluye el código de negocio.</summary>
public sealed class SwaggerExamplesOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        foreach (var response in operation.Responses ?? [])
        {
            if (!int.TryParse(response.Key, out var statusCode) || statusCode < 400)
            {
                continue;
            }

            // Swagger muestra el mismo contrato para cualquier 4xx/5xx; en tiempo de ejecución el handler completa el código exacto.
            var example = JsonNode.Parse("""
            {
              "title": "Error de dominio",
              "status": 400,
              "detail": "La solicitud no pudo procesarse.",
              "errorCode": "PRD-002",
              "errorMessage": "Los datos del producto son inválidos.",
              "correlationId": "b4f7d3c2-..."
            }
            """)!;

            foreach (var content in response.Value.Content?.Values ?? [])
            {
                if (content is OpenApiMediaType mediaType)
                {
                    mediaType.Example = example;
                }
            }
        }
    }
}
