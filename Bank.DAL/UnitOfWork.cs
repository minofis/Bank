using System.Data;
using Bank.Core.Interfaces;
using Bank.Core.Interfaces.Repositories;
using Bank.DAL.Repositories;

namespace Bank.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        public IAccountsRepository Accounts { get; private set; }
        public ITransactionsRepository Transactions { get; private set; }
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Accounts = new AccountsRepository(_context);
            Transactions = new TransactionsRepository(_context);
        }

        public async Task BeginTransactionAsync()
        {
            await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
            await _context.Database.CommitTransactionAsync();
        }

        public async Task RollbackAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}