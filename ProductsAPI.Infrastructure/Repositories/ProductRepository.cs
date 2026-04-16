using Microsoft.EntityFrameworkCore;
using ProductsAPI.Domain.Common;
using ProductsAPI.Domain.Entities;
using ProductsAPI.Infrastructure.Data;
using ProductsAPI.Infrastructure.Extensions;
using ProductsAPI.Infrastructure.Repositories.Interfaces;

namespace ProductsAPI.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context) => _context = context;

        public Task<PagedList<Product>> GetPagedAsync(int pageNumber, int pageSize) =>
            _context.Products.AsNoTracking().OrderBy(p => p.CreatedAt).ToPagedListAsync(pageNumber, pageSize);

        public async Task<Product?> GetByIdAsync(int id) =>
            await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

        public async Task<Product> CreateAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product is null) return false;

            product.IsDeleted = true;
            product.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
