using Bank.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bank.DAL.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder
                .HasOne(t => t.Type)
                .WithMany()
                .HasForeignKey(t => t.TypeId);

            builder
                .HasOne(t => t.SenderAccount)
                .WithMany(a => a.SentTransactions)
                .HasForeignKey(t => t.SenderAccountId);

            builder
                .HasOne(t => t.RecipientAccount)
                .WithMany(a => a.RecivedTransactions)
                .HasForeignKey(t => t.RecipientAccountId);
        }
    }
}