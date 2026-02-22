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
        public DbSet<CustomerMalls> CustomerMalls { get; set; }

        public DbSet<Student> Students { get; set; }
        public DbSet<Courses> Courses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                modelBuilder.Entity<Mall>().HasData(
                    new Mall { Id = 1, Name = "Sunshine Mall", Location = "Downtown", Floors = 5 },
                    new Mall { Id = 2, Name = "Riverfront Mall", Location = "Riverside", Floors = 3 },
                    new Mall { Id = 3, Name = "Mountainview Mall", Location = "Hillside", Floors = 4 }
                );

            modelBuilder.Entity<Student>().HasData(
                new Student { Id = 1, Name = "Alice" },
                new Student { Id = 2, Name = "Bob" },
                new Student { Id = 3, Name = "Charlie" }
                );

            modelBuilder.Entity<Courses>().HasData(
                new Courses { Id = 1, Name = "Mathematics", StudentId = 1 },
                new Courses { Id = 2, Name = "Physics", StudentId = 1 },
                new Courses { Id = 3, Name = "Chemistry",StudentId = 2 }
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

            modelBuilder.Entity<Shop>()
                .HasMany(s => s.Customers)
                .WithMany(m => m.Shops)
                .UsingEntity(j => j.ToTable("CustomerShops"));

            modelBuilder.Entity<CustomerMalls>()
                .HasKey(cm => new { cm.CustomerId, cm.MallId });

            modelBuilder.Entity<CustomerMalls>()
                .HasOne(cm => cm.Customer)
                .WithMany(c => c.CustomerMalls)
                .HasForeignKey(cm => cm.CustomerId);

            modelBuilder.Entity<CustomerMalls>()
                .HasOne(cm => cm.Mall)
                .WithMany(c => c.CustomerMalls)
                .HasForeignKey(cm => cm.MallId);

        }
    }
}
