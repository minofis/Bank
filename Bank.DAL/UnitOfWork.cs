using System.Data;
using Bank.Core.Interfaces;
using Bank.Core.Interfaces.Repositories;
using Bank.DAL.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Bank.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction _transaction;

        public IAccountsRepository Accounts { get; private set; }
        public ITransactionsRepository Transactions { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Accounts = new AccountsRepository(_context);
            Transactions = new TransactionsRepository(_context);
        }

        public async Task BeginTransactionAsync()
        {
            if (_transaction != null)
            {
                throw new InvalidOperationException("A transaction is already in progress");
            }
            
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            try
            {
                if (_transaction == null)
                {
                    throw new InvalidOperationException("No transaction to commit");
                }

                await _context.SaveChangesAsync();
                await _transaction.CommitAsync();
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackAsync()
        {
            try
            {
                if (_transaction != null)
                {
                    await _transaction.RollbackAsync();
                }
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}