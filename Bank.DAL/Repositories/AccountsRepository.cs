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

        public async Task CreateAccountAsync(Account account)
        {
            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();
        }

        public async Task<Account> GetAccountByIdAsync(Guid accountId)
        {
            return await _context.Accounts
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == accountId);
        }

        public async Task<List<Account>> GetAllAcountsAsync()
        {
            return await _context.Accounts
                .AsNoTracking()
                .ToListAsync();
        }
    }
}