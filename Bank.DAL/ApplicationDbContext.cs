using Microsoft.EntityFrameworkCore;

namespace Bank.DAL
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Entities
        // public DbSet<Wallet> Wallets { get; set; }
        // public DbSet<Profile> Profiles { get; set; }
        
        
    }
}