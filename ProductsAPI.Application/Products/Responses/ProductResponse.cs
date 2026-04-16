namespace ProductsAPI.Application.Products.Responses
{
    public record ProductResponse(int Id, string Name, string Description, decimal Price, int Stock, DateTime CreatedAt);
}
