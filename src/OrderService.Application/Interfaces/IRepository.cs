namespace OrderService.Application.Interfaces;

public interface IRepository<T>
{
    Task AddAsync(T entity);

    Task<List<T>> GetAllAsync();

    Task<T?> FindByIdAsync(Guid id);

    void Delete(T entity);
}