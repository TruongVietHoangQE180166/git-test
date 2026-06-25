using Microsoft.EntityFrameworkCore;
using TruyenCV.Application.Common.Interfaces;
using TruyenCV.Infrastructure.Persistence.Context;

namespace TruyenCV.Infrastructure.Persistence.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext Context;
    protected readonly DbSet<T> DbSet;

    public Repository(ApplicationDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await DbSet.FindAsync([id], ct);

    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
        => await DbSet.ToListAsync(ct);

    public virtual async Task AddAsync(T entity, CancellationToken ct = default)
        => await DbSet.AddAsync(entity, ct);

    public virtual void Update(T entity)
        => DbSet.Update(entity);

    public virtual void Delete(T entity)
        => DbSet.Remove(entity);

    public virtual async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await Context.SaveChangesAsync(ct);
}
