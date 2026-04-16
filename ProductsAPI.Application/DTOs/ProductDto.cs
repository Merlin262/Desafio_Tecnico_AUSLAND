using System;
using System.Collections.Generic;
using System.Text;

namespace ProductsAPI.Application.DTOs
{
    public record ProductDto(int Id, string Name, string Description, decimal Price, int Stock, DateTime CreatedAt);

    public record CreateProductDto(string Name, string Description, decimal Price, int Stock);

    public record UpdateProductDto(string Name, string Description, decimal Price, int Stock);
}
