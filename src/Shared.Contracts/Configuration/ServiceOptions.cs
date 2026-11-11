// [TP Microservicios E-Commerce v7] Centraliza contratos técnicos reutilizables para responder de forma consistente a la consigna.
namespace Shared.Contracts.Configuration;

public class ServiceOptions
{
    public const string SectionName = "Services";
    public string Products { get; init; } = "http://localhost:5101";
    public string Users { get; init; } = "http://localhost:5102";
    public string Orders { get; init; } = "http://localhost:5103";
    public string Cart { get; init; } = "http://localhost:5104";
    public string Notifications { get; init; } = "http://localhost:5105";
}
