
namespace Catalog.API.Products.UpdateProduct
{
    public record UpdateProductRequest(Product Product);
    public record UpdateProductResponse(Product Product);
    public class UpdateProductEndpoint() : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/products/{productId}", async (Guid productId, UpdateProductRequest productRequest, ISender sender) =>
            {
                try
                {
                    var command = new UpdateProductCommand(productId, productRequest.Product);

                    var result = await sender.Send(command);

                    var response = result.Adapt<UpdateProductResponse>();

                    return Results.Ok(response);
                }
                catch (KeyNotFoundException ex)
                {
                    return Results.NotFound(new { error = ex.Message });
                }                
                catch (InvalidOperationException ex)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            })
            .WithName("UpdateProduct")
            .Produces<UpdateProductResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Update product")
            .WithDescription("Updates a product from the catalog based on the given ID.");
        }
    }
}