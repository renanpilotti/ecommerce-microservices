namespace Catalog.API.Products.GetProductById
{
    public record GetProductByIdQuery(Guid ProductId) : IQuery<GetProductByIdQueryResult>;
    public record GetProductByIdQueryResult(Product Product);
    public class GetProductByIdQueryHandler(IDocumentSession session) : IQueryHandler<GetProductByIdQuery, GetProductByIdQueryResult>
    {
        public async Task<GetProductByIdQueryResult> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
        {
            var product = await session.LoadAsync<Product>(query.ProductId, cancellationToken); 

            if (product == null)
                throw new NotFoundException("Product", query.ProductId);

            return new GetProductByIdQueryResult(product);
        }
    }
}
