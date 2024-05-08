
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
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Product entity)
        {
            _context.Products.Remove(entity);
            await _context.SaveChangesAsync();
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
            _context.Products.Update(entity);
            await _context.SaveChangesAsync();
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