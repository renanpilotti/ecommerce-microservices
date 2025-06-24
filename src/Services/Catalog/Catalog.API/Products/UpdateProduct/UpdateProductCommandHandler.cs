
namespace Catalog.API.Products.UpdateProduct
{
    public record UpdateProductCommand(Guid ProductId, Product Product) : ICommand<UpdateProductResult>;
    public record UpdateProductResult(Product Product);
    public class UpdateProductCommandHandler(IDocumentSession session) : ICommandHandler<UpdateProductCommand, UpdateProductResult>
    {
        public async Task<UpdateProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            if (command.ProductId != command.Product.Id) 
                throw new BadRequestException($"The ProductId in the route ({command.ProductId}) does not match the ProductId in the body ({command.Product.Id}).");

            var product = await session.LoadAsync<Product>(command.ProductId, cancellationToken);

            if (product == null) 
                throw new NotFoundException("Product", command.ProductId);

            product.Name = command.Product.Name;
            product.Description = command.Product.Description;
            product.Category = command.Product.Category;
            product.ImageFile = command.Product.ImageFile;
            product.Price = command.Product.Price;

            session.Update(product);
            await session.SaveChangesAsync(cancellationToken);

            return new UpdateProductResult(product);
        }
    }
}
