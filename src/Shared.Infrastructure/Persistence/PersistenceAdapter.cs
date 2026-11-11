// [TP Microservicios E-Commerce v7] Agrupa infraestructura transversal reutilizable: errores, logs, persistencia y correlación.
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.Sqlite;

namespace Shared.Infrastructure.Persistence;

/// <summary>Oculta la tecnología de almacenamiento para que los Services mantengan sus reglas de negocio.</summary>
public interface IEntityStore<T>
{
    IReadOnlyCollection<T> Load();
    void Save(IReadOnlyCollection<T> entities);
}

/// <summary>Implementación simple para pruebas unitarias sin abrir una base de datos.</summary>
public class InMemoryEntityStore<T> : IEntityStore<T>
{
    private List<T> _entities = [];
    public IReadOnlyCollection<T> Load() => _entities.AsReadOnly();
    public void Save(IReadOnlyCollection<T> entities) => _entities = entities.ToList();
}

/// <summary>Persistencia SQLite inspirada en la implementación de referencia de la cátedra.</summary>
public class SqliteEntityStore<T>(IConfiguration configuration) : IEntityStore<T>
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "Data Source=ecommerce.db";
    private readonly string _storeName = typeof(T).FullName ?? typeof(T).Name;

    public IReadOnlyCollection<T> Load()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        EnsureTable(connection);
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT data FROM entity_store WHERE name = $name";
        command.Parameters.AddWithValue("$name", _storeName);
        var json = command.ExecuteScalar() as string;
        return string.IsNullOrWhiteSpace(json) ? [] : JsonSerializer.Deserialize<List<T>>(json) ?? [];
    }

    public void Save(IReadOnlyCollection<T> entities)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        EnsureTable(connection);
        using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO entity_store(name, data) VALUES($name, $data) ON CONFLICT(name) DO UPDATE SET data = excluded.data";
        command.Parameters.AddWithValue("$name", _storeName);
        command.Parameters.AddWithValue("$data", JsonSerializer.Serialize(entities));
        command.ExecuteNonQuery();
    }

    private static void EnsureTable(SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "CREATE TABLE IF NOT EXISTS entity_store (name TEXT PRIMARY KEY, data TEXT NOT NULL)";
        command.ExecuteNonQuery();
    }
}
