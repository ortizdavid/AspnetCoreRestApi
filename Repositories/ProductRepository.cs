
using AspNetCoreRestApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreRestApi.Repositories
{
    public class ProductRepository : IRepository<Product>
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Product entity)
        {
            try
            {
                await _context.Products.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task CreateBatchAsync(List<Product> entities)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await _context.Products.AddRangeAsync(entities);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task DeleteAsync(Product entity)
        {
            try
            {
                _context.Products.Remove(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<List<ProductData>> GetAllDataAsync()
        {
            return await _context.ProductData.ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }
        
        public async Task<Product?> GetByCodeAsync(string? predicate)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.Code == predicate);    
        }

        public async Task<ProductData?> GetDataByIdAsync(int id)
        {
            return await _context.ProductData.FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task<Product?> GetByUniqueIdAsync(Guid uniqueId)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.UniqueId == uniqueId);
        }

        public async Task UpdateAsync(Product entity)
        {
            try
            {
                _context.Products.Update(entity);
                await _context.SaveChangesAsync();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task<bool> ExistsAsync(string? predicate)
        {
            if (predicate == null)
                return false;
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Code == predicate);
            return product != null;
        }

    }
}