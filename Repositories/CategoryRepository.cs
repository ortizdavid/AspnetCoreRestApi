using System.Data;
using AspNetCoreRestApi.Models;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreRestApi.Repositories
{
    public class CategoryRepository : IRepository<Category>
    {
        private readonly AppDbContext _context;
        private IDbConnection _dapper;

        public CategoryRepository(AppDbContext context, IDbConnection dapper)
        {
            _context = context;
            _dapper = dapper;
        }

        public async Task CreateAsync(Category entity)
        {
            try
            {
                await _context.Categories.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task CreateBatchAsync(List<Category> entities)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await _context.Categories.AddRangeAsync(entities);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public async Task UpdateAsync(Category entity)
        {
            try
            {
                _context.Categories.Update(entity);
                await _context.SaveChangesAsync();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task DeleteAsync(Category entity)
        {
            try
            {
                _context.Categories.Remove(entity);
                await _context.SaveChangesAsync();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task<List<Category>> GetAllAsync(int limit, int offset)
        {
            return await _context.Categories
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            var sql = "SELECT COUNT(*) FROM Categories;";
            return await _dapper.ExecuteScalarAsync<int>(sql);
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task<Category?> GetByUniqueIdAsync(Guid uniqueId)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.UniqueId == uniqueId);
        }

        public async Task<bool> ExistsAsync(string? predicate)
        {
            if (string.IsNullOrEmpty(predicate))
            {
                return false;
            } 
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryName == predicate);
            return category != null;
        }

        public async Task<Category?> GetByNameAsync(string? name)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryName == name);
        }
    }
}