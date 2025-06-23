
namespace Catalog.API.Products.DeleteProduct
{
    public record DeleteProductCommand(Guid ProductId) : ICommand<DeleteProductResult>;
    public record DeleteProductResult(Product Product);
    public class DeleteProductCommandHandler(IDocumentSession session) : ICommandHandler<DeleteProductCommand, DeleteProductResult>
    {
        public async Task<DeleteProductResult> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
        {
            var product = await session.LoadAsync<Product>(command.ProductId, cancellationToken);

            if (product == null) throw new KeyNotFoundException($"Product with ID '{command.ProductId}' was not found.");

            session.Delete<Product>(command.ProductId);
            await session.SaveChangesAsync(cancellationToken);

            return new DeleteProductResult(product);
        }
    }
}
