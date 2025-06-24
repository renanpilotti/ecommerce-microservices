
namespace Catalog.API.Products.GetProductById
{
    public record GetProductByIdResponse(Product Product);
    public class GetProductByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("products/{productId}", async (Guid productId, ISender sender) =>
            {
                var result = await sender.Send(new GetProductByIdQuery(productId));
                var response = result.Adapt<GetProductByIdResponse>();
                return Results.Ok(response.Product);
            })
            .WithName("GetProduct")
            .Produces<GetProductByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get a single product")
            .WithDescription("Gets a product from the catalog based on the given ID.");
        }
    }
}
