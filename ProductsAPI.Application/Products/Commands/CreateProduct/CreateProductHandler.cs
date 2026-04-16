using ProductsAPI.Application.DTOs;
using ProductsAPI.Application.Interfaces;
using ProductsAPI.Domain.Entities;
using ProductsAPI.Infrastructure.Repositories.Interfaces;

namespace ProductsAPI.Application.Products.Commands.CreateProduct
{
    public class CreateProductHandler
    {
        private readonly IProductRepository _repository;

        public CreateProductHandler(IProductRepository repository) => _repository = repository;

        public async Task<ProductDto> Handle(CreateProductCommand command)
        {
            var product = new Product
            {
                Name = command.Name,
                Description = command.Description,
                Price = command.Price,
                Stock = command.Stock
            };

            var created = await _repository.CreateAsync(product);

            return new ProductDto(
                created.Id, created.Name, created.Description,
                created.Price, created.Stock, created.CreatedAt);
        }
    }
}
