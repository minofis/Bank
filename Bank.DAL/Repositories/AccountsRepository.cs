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

        public async Task<List<Account>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Accounts
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task CreateAsync(Account account, CancellationToken ct = default)
        {
            await _context.Accounts.AddAsync(account, ct);
            await _context.SaveChangesAsync(ct).ConfigureAwait(false);
        }

        public async Task<Account> GetByNumberAsync(string accountNumber, CancellationToken ct = default)
        {
            return await _context.Accounts
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber, ct);
        }

        // Operation to avoid "data races" during transactions
        public async Task<Account> GetByNumberLockedAsync(string accountNumber, CancellationToken ct = default)
        {
            if (await _context.Accounts.AnyAsync(a => a.AccountNumber == accountNumber, ct))
            {
                return await _context.Accounts
                .FromSqlInterpolated($"SELECT * FROM accounts WHERE account_number = {accountNumber} FOR UPDATE")
                .AsTracking()
                .FirstOrDefaultAsync(ct);
            }
            return null;
        }
    }
}