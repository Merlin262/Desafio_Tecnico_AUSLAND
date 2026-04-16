using ProductsAPI.Application.Interfaces;
using ProductsAPI.Domain.Entities;
using ProductsAPI.Infrastructure.Repositories.Interfaces;

namespace ProductsAPI.Application.Products.Commands.UpdateProduct
{
    public class UpdateProductHandler
    {
        private readonly IProductRepository _repository;

        public UpdateProductHandler(IProductRepository repository) => _repository = repository;

        // Wolverine entende NotFound automaticamente quando retorna null
        public async Task<bool> Handle(UpdateProductCommand command)
        {
            var existing = await _repository.GetByIdAsync(command.Id);
            if (existing is null) return false;

            var updated = new Product
            {
                Id = existing.Id,
                Name = command.Name,
                Description = command.Description,
                Price = command.Price,
                Stock = command.Stock,
                CreatedAt = existing.CreatedAt,
                UpdatedAt = DateTime.UtcNow
            };

            await _repository.UpdateAsync(updated);
            return true;
        }
    }
}
