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
        }
    }
}
