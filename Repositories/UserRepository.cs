using System.Data;
using AspNetCoreRestApi.Models;
using Microsoft.EntityFrameworkCore;
using Dapper;

namespace AspNetCoreRestApi.Repositories
{
    public class UserRepository //: IRepository<User>
    {
        private readonly AppDbContext _context;
        private readonly IDbConnection _dapper;

        public UserRepository(AppDbContext context, IDbConnection dapper)
        {
            _context = context;
            _dapper = dapper;
        }

        public async Task CreateAsync(User entity)
        {
           try
           {
                await _context.Users.AddAsync(entity);
                await _context.SaveChangesAsync();
           }
           catch (Exception)
           {
                throw;
           }
        }

        public async Task CreateBatchAsync(List<User> entities)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await _context.AddRangeAsync(entities);
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

        public async Task DeleteAsync(User entity)
        {
            try
            {
                _context.Remove(entity);
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
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == predicate);
            return user != null;
        }

        public async Task<List<User>> GetAllAsync(int limit, int offset)
        {
            return await _context.Users
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            var sql = "SELECT COUNT(*) FROM Users;";
            return await _dapper.ExecuteScalarAsync<int>(sql);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetByUserNameAsync(string? userName)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public User? GetByUserName(string? userName)
        {
            return _context.Users.FirstOrDefault(u => u.UserName == userName);
        }

        public async Task<User?> GetByUniqueIdAsync(Guid uniqueId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UniqueId == uniqueId);
        }

        public async Task UpdateAsync(User entity)
        {
            try
            {
                _context.Update(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}