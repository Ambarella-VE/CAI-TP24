// [TP Microservicios E-Commerce v7] Configura o respalda una parte necesaria de Shared.Tests para cumplir la consigna.
using Shared.Contracts.Errors;
using Shared.Infrastructure.Health;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Shared.Infrastructure.Errors;
using Shared.Infrastructure.Correlation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Shared.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;

namespace Shared.Tests;

public class ErrorCatalogTests
{
    [Fact]
    public void Find_debe_devolver_los_datos_de_PRD_001()
    {
        var error = ErrorCatalog.Find("PRD-001");

        Assert.Equal("PRD-001", error.Code);
        Assert.Equal(404, error.StatusCode);
        Assert.Equal("Producto no encontrado.", error.Message);
    }

    [Fact]
    public void Find_debe_devolver_error_de_regla_para_ORD_005()
    {
        var error = ErrorCatalog.Find("ORD-005");

        Assert.Equal(422, error.StatusCode);
        Assert.Equal(ErrorCategory.BusinessRule, error.Category);
    }

    [Fact]
    public void DomainException_debe_usar_los_metadatos_del_catálogo()
    {
        var exception = new DomainException("CRT-004");

        Assert.Equal("CRT-004", exception.ErrorCode);
        Assert.Equal(400, exception.StatusCode);
        Assert.Equal(ErrorCategory.Validation, exception.Category);
        Assert.Equal("Cantidad inválida.", exception.Message);
    }

    [Fact]
    public void ApiProblemDetails_debe_incluir_error_y_correlación()
    {
        var problem = new ApiProblemDetails(new DomainException("USR-003"), "correlation-123");

        Assert.Equal(401, problem.Status);
        Assert.Equal("USR-003", problem.ErrorCode);
        Assert.Equal("correlation-123", problem.CorrelationId);
    }

    [Fact]
    public async Task DomainExceptionHandler_debe_devolver_el_status_del_catálogo()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.Request.Headers["X-Correlation-Id"] = "corr-1";
        var handler = new DomainExceptionHandler();

        var handled = await handler.TryHandleAsync(context, new DomainException("PRD-001"), CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(404, context.Response.StatusCode);
    }

    [Fact]
    public async Task CorrelationIdMiddleware_debe_crear_un_header_si_no_llega_en_el_request()
    {
        var context = new DefaultHttpContext();
        var middleware = new CorrelationIdMiddleware(_ => Task.CompletedTask);

        await middleware.InvokeAsync(context);

        Assert.True(context.Response.Headers.ContainsKey("X-Correlation-Id"));
        Assert.False(string.IsNullOrWhiteSpace(context.Response.Headers["X-Correlation-Id"]));
    }
}

public class HealthCheckResponseWriterTests
{
    [Fact]
    public async Task Write_debe_devolver_estado_en_JSON()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var report = new HealthReport(new Dictionary<string, HealthReportEntry>(), HealthStatus.Healthy, TimeSpan.Zero);

        await HealthCheckResponseWriter.Write(context, report);

        context.Response.Body.Position = 0;
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        Assert.Contains("Healthy", body);
        Assert.StartsWith("application/json", context.Response.ContentType);
    }
}

public class SqliteEntityStoreTests
{
    [Fact]
    public void Save_y_Load_deben_conservar_los_datos_del_tipo()
    {
        var databasePath = Path.Combine(Path.GetTempPath(), $"ecommerce-test-{Guid.NewGuid()}.db");
        try
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = $"Data Source={databasePath}"
                })
                .Build();
            var store = new SqliteEntityStore<PersistedExample>(configuration);

            store.Save([new PersistedExample("dato guardado")]);

            var recovered = store.Load();
            Assert.Single(recovered);
            Assert.Equal("dato guardado", recovered.Single().Name);
        }
        finally
        {
            // SQLite agrupa conexiones para rendimiento; en una prueba liberamos ese pool antes de borrar su archivo temporal.
            SqliteConnection.ClearAllPools();
            if (File.Exists(databasePath)) File.Delete(databasePath);
        }
    }

    private sealed record PersistedExample(string Name);
}
