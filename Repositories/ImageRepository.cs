using AspNetCoreRestApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreRestApi.Repositories
{
    public class ImageRepository
    {
        protected readonly AppDbContext _context;

        public ImageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Image image)
        {
            try
            {
                await _context.Images.AddAsync(image);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {   
                throw;
            }
        }

        public async Task CreateBatchAsync(Image image)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await _context.Images.AddAsync(image);
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

        public async Task DeleteAsync(int productId)
        {
            try
            {
                var images = await _context.Images
                        .Where(img => img.ProductId == productId)
                        .ToListAsync();
                _context.Images.RemoveRange(images);
                await _context.SaveChangesAsync(); 
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Image>> GetAllAsync(int productId)
        {
            var images = await _context.Images
                    .Where(img => img.ProductId == productId)
                    .ToListAsync();
            return images;
        }
    }
}