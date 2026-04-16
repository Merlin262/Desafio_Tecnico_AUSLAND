namespace ProductsAPI.Application.Products.Commands.UpdateProduct
{
    public record UpdateProductCommand(
        int Id,
        string Name,
        string Description,
        decimal Price,
        int Stock);
}
