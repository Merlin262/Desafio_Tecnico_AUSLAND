using System.Linq;
using ProductsAPI.Application.Common;
using ProductsAPI.Application.DTOs;
using ProductsAPI.Application.Interfaces;
using ProductsAPI.Domain.Entities;
using ProductsAPI.Infrastructure.Repositories.Interfaces;

namespace ProductsAPI.Application.Products.Queries.GetAllProducts
{
    public class GetAllProductsHandler
    {
        private readonly IProductRepository _repository;

        public GetAllProductsHandler(IProductRepository repository) => _repository = repository;

        public async Task<PagedResult<ProductDto>> Handle(GetAllProductsQuery query)
        {
            var paged = await _repository.GetPagedAsync(query.PageNumber, query.PageSize);
            var dtos = paged.Items.Select(p => new ProductDto(
                p.Id, p.Name, p.Description, p.Price, p.Stock, p.CreatedAt));

            return new PagedResult<ProductDto>(dtos, paged.TotalCount, paged.PageNumber, paged.PageSize);
        }
    }
}
