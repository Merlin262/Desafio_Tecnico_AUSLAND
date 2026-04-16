using Microsoft.EntityFrameworkCore;
using ProductsAPI.Domain.Entities;

namespace ProductsAPI.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // filtro global — IsDeleted nunca aparece nas queries
            modelBuilder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);

            modelBuilder.Entity<Product>().Property(p => p.Price)
                .HasColumnType("decimal(18,2)");
        }
    }
}
