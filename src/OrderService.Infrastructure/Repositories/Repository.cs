using Microsoft.EntityFrameworkCore;
using OrderService.Application.Interfaces;
using OrderService.Infrastructure.Data;
using OrderService.Application.Specifications;
using Domain.Common;
namespace OrderService.Infrastructure.Repositories;


public class Repository<T> : IRepository<T>
    where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

   public async Task<List<T>> GetAllAsync()
{
    var query = _dbSet.AsQueryable();

    if (typeof(ISoftDeletable).IsAssignableFrom(typeof(T)))
    {
        query = query.Where(x =>
            !EF.Property<bool>(x, nameof(ISoftDeletable.IsDeleted)));
    }

    return await query.ToListAsync();
}

    public async Task<T?> FindByIdAsync(Guid id)
{
    var query = _dbSet.AsQueryable();

    if (typeof(ISoftDeletable).IsAssignableFrom(typeof(T)))
    {
        query = query.Where(x =>
            !EF.Property<bool>(x, nameof(ISoftDeletable.IsDeleted)));
    }

    return await query.FirstOrDefaultAsync(
        x => EF.Property<Guid>(x, "Id") == id);
}

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }
    public async Task<List<T>> GetBySpecificationAsync(
    ISpecification<T> specification)
{
    var query = _dbSet.AsQueryable();

    if (typeof(ISoftDeletable).IsAssignableFrom(typeof(T)))
    {
        query = query.Where(x =>
            !EF.Property<bool>(x, nameof(ISoftDeletable.IsDeleted)));
    }

    if (specification.Criteria != null)
    {
        query = query.Where(specification.Criteria);
    }

    return await query.ToListAsync();
}

}