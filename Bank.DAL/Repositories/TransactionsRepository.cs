using Bank.Core.Entities;
using Bank.Core.Interfaces.Repositories;

namespace Bank.DAL.Repositories
{
    public class TransactionsRepository : ITransactionsRepository
    {
        private readonly ApplicationDbContext _context;
        public TransactionsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
        }
    }
}