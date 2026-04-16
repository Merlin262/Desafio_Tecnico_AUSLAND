namespace ProductsAPI.Application.Products.Queries.GetAllProducts
{
    public record GetAllProductsQuery(int PageNumber = 1, int PageSize = 10);
}
