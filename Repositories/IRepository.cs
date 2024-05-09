

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCoreRestApi.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task CreateAsync(T entity);
        Task CreateBatchAsync(List<T> entities);
        Task DeleteAsync(T entity);
        Task<List<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<T?> GetByUniqueIdAsync(Guid uniqueId);
        Task UpdateAsync(T entity);
        Task<bool> ExistsAsync(string predicate);
    }
}
