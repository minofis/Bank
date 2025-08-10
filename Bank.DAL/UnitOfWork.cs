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
        private bool _disposed;

        public IAccountsRepository Accounts { get; private set; }
        public ITransactionsRepository Transactions { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Accounts = new AccountsRepository(_context);
            Transactions = new TransactionsRepository(_context);
        }

        public async Task BeginTransactionAsync(CancellationToken ct = default)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(UnitOfWork));

            if (_transaction != null)
                throw new InvalidOperationException("A transaction is already in progress.");

            _transaction = await _context.Database.BeginTransactionAsync(ct);
        }

        public async Task CommitAsync(CancellationToken ct = default)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(UnitOfWork));

            if (_transaction == null)
                throw new InvalidOperationException("No transaction to commit.");
            try
            {
                await _context.SaveChangesAsync(ct);
                await _transaction.CommitAsync(ct);
            }
            catch
            {
                await TryRollbackAsync(ct);
                throw;
            }
            finally
            {
                await TryDisposeTransactionAsync();
            }
        }

        public async Task RollbackAsync(CancellationToken ct = default)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(UnitOfWork));

            await TryRollbackAsync(ct);
            await TryDisposeTransactionAsync();
        }

        private async Task TryRollbackAsync(CancellationToken ct = default)
        {
            try
            {
                if (_transaction != null)
                    await _transaction.RollbackAsync(ct);
            }
            catch
            {
                // Log the error but do not throw an exception
            }
        }

        private async Task TryDisposeTransactionAsync()
        {
            if (_transaction != null && !_disposed)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed)
                return;

            try
            {
                await TryRollbackAsync();
                await TryDisposeTransactionAsync();
                await _context.DisposeAsync();
            }
            finally
            {
                _disposed = true;
            }
        }
        
        public void Dispose()
        {
            DisposeAsync().AsTask().GetAwaiter().GetResult();
        }
    }
}