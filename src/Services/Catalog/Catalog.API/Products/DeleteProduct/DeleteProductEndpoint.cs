
namespace Catalog.API.Products.DeleteProduct
{
    public record DeleteProductResponse(Product Product);
    public class DeleteProductEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/products/{productId}", async (Guid productId, ISender sender) =>
            {
                var deletedProduct = await sender.Send(new DeleteProductCommand(productId));

                var response = deletedProduct.Adapt<DeleteProductResponse>();

                return Results.Ok(response);
            })
            .WithName("DeleteProduct")
            .Produces<DeleteProductResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete a single product")
            .WithDescription("Removes a product from the catalog based on the given ID.");
        }
    }
}
