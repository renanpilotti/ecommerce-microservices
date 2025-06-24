namespace Catalog.API.Products.CreateProduct
{
    public record CreateProductRequest(
        string Name,
        string Description,
        string ImageFile,
        decimal Price,
        List<string> Category
    );
    public record CreateProductResponse(Guid ProductId);
    public class CreateProductEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/products", async (CreateProductRequest request, ISender sender) =>
            {
                var command = request.Adapt<CreateProductCommand>();

                var result = await sender.Send(command);

                var response = result.Adapt<CreateProductResponse>();

                return Results.Created($"/products/{response.ProductId}", response);

            })
            .WithName("CreateProduct")
            .Produces<CreateProductResponse>(StatusCodes.Status201Created)
            .Produces<CreateProductResponse>(StatusCodes.Status400BadRequest)
            .WithSummary("Create product")
            .WithDescription("Receives a valid product payload and persists a new product entity.");
        }
    }
}
