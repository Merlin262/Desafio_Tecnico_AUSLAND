using ProductsAPI.Application.Interfaces;
using ProductsAPI.Infrastructure.Repositories.Interfaces;

namespace ProductsAPI.Application.Products.Commands.DeleteProduct
{
    public class DeleteProductHandler
    {
        private readonly IProductRepository _repository;

        public DeleteProductHandler(IProductRepository repository) => _repository = repository;

        public async Task<bool> Handle(DeleteProductCommand command) =>
            await _repository.DeleteAsync(command.Id);
    }
}
