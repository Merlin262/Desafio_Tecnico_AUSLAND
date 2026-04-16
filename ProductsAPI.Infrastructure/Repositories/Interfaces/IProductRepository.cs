using ProductsAPI.Domain.Common;
using ProductsAPI.Domain.Entities;

namespace ProductsAPI.Infrastructure.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<PagedList<Product>> GetPagedAsync(int pageNumber, int pageSize);
        Task<Product?> GetByIdAsync(int id);
        Task<Product> CreateAsync(Product product);
        Task UpdateAsync(Product product);
        Task<bool> DeleteAsync(int id);
    }
}
