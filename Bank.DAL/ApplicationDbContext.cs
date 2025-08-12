using Bank.Core.Entities;
using Bank.Core.Lookups;
using Bank.DAL.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Bank.DAL
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Entities
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        // Lookups
        public DbSet<TransactionType> TransactionTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Entities
            modelBuilder.ApplyConfiguration(new TransactionConfiguration());

            // Lookups
            modelBuilder.ApplyConfiguration(new TransactionTypeConfiguration());
            
            base.OnModelCreating(modelBuilder);
        }
    }
}