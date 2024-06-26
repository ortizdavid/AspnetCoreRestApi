using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCoreRestApi.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task CreateAsync(T entity);
        Task CreateBatchAsync(List<T> entities);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<List<T>> GetAllAsync(int limit, int offset);
        Task<T?> GetByIdAsync(int id);
        Task<T?> GetByUniqueIdAsync(Guid uniqueId);
        Task<bool> ExistsAsync(string? predicate);
    }
}
