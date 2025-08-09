using Bank.Core.Entities;
using Bank.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Bank.DAL.Repositories
{
    public class AccountsRepository : IAccountsRepository
    {
        private readonly ApplicationDbContext _context;
        public AccountsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Account>> GetAllAsync()
        {
            return await _context.Accounts
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task CreateAsync(Account account)
        {
            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();
        }

        public async Task<Account> GetByNumberAsync(string accountNumber)
        {
            return await _context.Accounts
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Number == accountNumber);
        }

        public async Task<Account> GetByNumberLockedAsync(string accountNumber)
        {
            return await _context.Accounts
                .FromSqlInterpolated($"SELECT * FROM Accounts WHERE Number = {accountNumber} FOR UPDATE")
                .AsTracking()
                .FirstOrDefaultAsync();
        }
    }
}