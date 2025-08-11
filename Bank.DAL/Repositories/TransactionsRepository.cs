using Bank.Core.Entities;
using Bank.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Bank.DAL.Repositories
{
    public class TransactionsRepository : ITransactionsRepository
    {
        private readonly ApplicationDbContext _context;
        public TransactionsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Transaction>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Transactions
                .Include(t => t.Type)
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task AddAsync(Transaction transaction, CancellationToken ct = default)
        {
            await _context.Transactions.AddAsync(transaction, ct);
        }
    }
}