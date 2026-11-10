namespace Shared.Infrastructure.Persistence;

public interface IEntityStore<T>
{
    void Add(T entity);
    IReadOnlyCollection<T> GetAll();
}

// Este adaptador mantiene el resto de la solución aislado de la tecnología de persistencia.
// En esta primera entrega usa memoria y puede reemplazarse por la librería de la cátedra.
public class InMemoryEntityStore<T> : IEntityStore<T>
{
    private readonly List<T> _entities = [];

    public void Add(T entity) => _entities.Add(entity);

    public IReadOnlyCollection<T> GetAll() => _entities.AsReadOnly();
}
