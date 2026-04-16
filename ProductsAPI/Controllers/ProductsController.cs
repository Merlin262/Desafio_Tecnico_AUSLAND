using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductsAPI.Application.DTOs;
using ProductsAPI.Application.Products.Commands.CreateProduct;
using ProductsAPI.Application.Products.Commands.DeleteProduct;
using ProductsAPI.Application.Products.Commands.UpdateProduct;
using ProductsAPI.Application.Products.Queries;
using ProductsAPI.Application.Products.Queries.GetAllProducts;
using ProductsAPI.Application.Products.Queries.GetProductById;
using Wolverine;

namespace ProductsAPI.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IMessageBus _bus;

        public ProductsController(IMessageBus bus) => _bus = bus;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10) =>
            Ok(await _bus.InvokeAsync<ProductsAPI.Application.Common.PagedResult<ProductDto>>(new GetAllProductsQuery(pageNumber, pageSize)));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _bus.InvokeAsync<ProductDto?>(new GetProductByIdQuery(id));
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
        {
            var created = await _bus.InvokeAsync<ProductDto>(command);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductCommand command)
        {
            var success = await _bus.InvokeAsync<bool>(command with { Id = id });
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _bus.InvokeAsync<bool>(new DeleteProductCommand(id));
            return success ? NoContent() : NotFound();
        }
    }
}
