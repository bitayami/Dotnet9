using Dotnet9.Models;
using Microsoft.EntityFrameworkCore;

namespace Dotnet9.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Mall> Malls { get; set; }
        public DbSet<Shop> Shops { get; set; }
        public DbSet<MallOwner> MallOwners { get; set; }

        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                modelBuilder.Entity<Mall>().HasData(
                    new Mall { Id = 1, Name = "Sunshine Mall", Location = "Downtown", Floors = 5 },
                    new Mall { Id = 2, Name = "Riverfront Mall", Location = "Riverside", Floors = 3 },
                    new Mall { Id = 3, Name = "Mountainview Mall", Location = "Hillside", Floors = 4 }
                );

            modelBuilder.Entity<Mall>()
                .HasOne(m => m.MallOwner)
                .WithOne(mo => mo.Mall)
                .HasForeignKey<MallOwner>(mo => mo.MallId);

            modelBuilder.Entity<Mall>()
                .HasMany(m => m.Shops)
                .WithOne(mo => mo.Mall)
                .HasForeignKey(s => s.MallId);

            modelBuilder.Entity<Mall>()
                .HasMany(m => m.Customers)
                .WithMany(c => c.Malls)
                .UsingEntity(j => j.ToTable("MallCustomers"));
        }
    }
}
