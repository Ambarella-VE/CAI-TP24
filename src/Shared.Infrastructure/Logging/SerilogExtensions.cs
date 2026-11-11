// [TP Microservicios E-Commerce v7] Agrupa infraestructura transversal reutilizable: errores, logs, persistencia y correlación.
using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Formatting.Compact;

namespace Shared.Infrastructure.Logging;

public static class SerilogExtensions
{
    public static WebApplicationBuilder AddSharedSerilog(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            // Identifica el proceso que emitió el evento cuando hay cinco APIs ejecutándose.
            .Enrich.WithProperty("Servicio", builder.Environment.ApplicationName)
            .Enrich.FromLogContext()
            // La consola queda legible durante desarrollo; el archivo conserva JSON para consultas estructuradas.
            .WriteTo.Console()
            .WriteTo.File(new RenderedCompactJsonFormatter(), "logs/log-.json", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        builder.Host.UseSerilog();
        return builder;
    }
}
