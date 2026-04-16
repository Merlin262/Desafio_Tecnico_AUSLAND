using ProductsAPI.Application.DTOs;
using ProductsAPI.Application.Interfaces;
using ProductsAPI.Application.Products.Responses;
using ProductsAPI.Domain.Entities;
using ProductsAPI.Infrastructure.Repositories.Interfaces;

namespace ProductsAPI.Application.Products.Queries.GetProductById
{
    public class GetProductByIdHandler
    {
        private readonly IProductRepository _repository;

        public GetProductByIdHandler(IProductRepository repository) => _repository = repository;

        public async Task<ProductDto?> Handle(GetProductByIdQuery query)
        {
            var product = await _repository.GetByIdAsync(query.Id);
            if (product is null) return null;

            return new ProductDto(
                product.Id, product.Name, product.Description,
                product.Price, product.Stock, product.CreatedAt);
        }
    }
}
