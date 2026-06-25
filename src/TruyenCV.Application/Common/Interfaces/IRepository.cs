using TruyenCV.Domain.Entities;

namespace TruyenCV.Application.Common.Interfaces;

/// <summary>
/// Generic repository abstraction for basic CRUD operations.
/// </summary>
/// <typeparam name="T">The domain entity type.</typeparam>
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);

    Task AddAsync(T entity, CancellationToken ct = default);

    void Update(T entity);

    void Delete(T entity);

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
