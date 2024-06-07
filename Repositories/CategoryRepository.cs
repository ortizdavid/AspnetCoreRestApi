using AspNetCoreRestApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreRestApi.Repositories
{
    public class CategoryRepository : IRepository<Category>
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
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

        public async Task<List<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
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