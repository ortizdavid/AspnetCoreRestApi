using AspNetCoreRestApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreRestApi.Repositories
{
    public class SupplierRepository : IRepository<Supplier>
    {
        private readonly AppDbContext _context;

        public SupplierRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Supplier entity)
        {
            try
            {
                await _context.Suppliers.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            };
        }

        public async Task CreateBatchAsync(List<Supplier> entities)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await _context.Suppliers.AddRangeAsync(entities);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                };
            }
        }

        public async Task UpdateAsync(Supplier entity)
        {
            try
            {
                _context.Suppliers.Update(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            };
        }

        public async Task DeleteAsync(Supplier entity)
        {
            try
            {
                _context.Suppliers.Remove(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> ExistsAsync(string? predicate)
        {
            if (predicate == null)
            {
                return false;
            }
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.IdentificationNumber == predicate);
            return supplier != null;   
        }

        public async Task<List<Supplier>> GetAllAsync()
        {
            return await _context.Suppliers.ToListAsync();
        }

        public async Task<Supplier?> GetByIdAsync(int id)
        {
            return await _context.Suppliers.FindAsync(id);
        }

        public async Task<Supplier?> GetByUniqueIdAsync(Guid uniqueId)
        {
            return await _context.Suppliers
                .FirstOrDefaultAsync(s => s.UniqueId == uniqueId);
        }

        public async Task<Supplier?> GetByIdentNumberAsync(string? identNumber)
        {
            return await _context.Suppliers
                .FirstOrDefaultAsync(s => s.IdentificationNumber == identNumber);    
        }

        public async Task<bool> ExistsRecord(string fieldName, string value)
        {
            if (string.IsNullOrEmpty(fieldName) || string.IsNullOrEmpty(value))
            {
                return false;
            }
            var validFieldNames = new List<string> { "primary_phone", "secondary_phone", "email" };
            if (!validFieldNames.Contains(fieldName))
            {
                throw new ArgumentException("Invalid field name");
            }
            var sql = $"SELECT 1 FROM Suppliers WHERE {fieldName} = @value LIMIT 1";
            var exists = await _context.Suppliers
                .FromSqlRaw(sql, new Npgsql.NpgsqlParameter("@value", value))
                .AnyAsync();
            return exists;
        }


    }
}