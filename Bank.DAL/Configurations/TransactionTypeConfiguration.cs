using Bank.Core.Enums;
using Bank.Core.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bank.DAL.Configurations
{
    public class TransactionTypeConfiguration : IEntityTypeConfiguration<TransactionType>
    {
        public void Configure(EntityTypeBuilder<TransactionType> builder)
        {
            builder
                .HasData(
                    Enum.GetValues(typeof(TransactionTypes))
                        .Cast<TransactionTypes>()
                        .Select(e => new TransactionType
                        {
                            Id = (int)e,
                            Name = e.ToString()
                        })
                );
        }
    }
}