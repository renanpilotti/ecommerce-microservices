namespace Catalog.API.Products.UpdateProduct
{
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(a => a.Product.Id).NotEmpty().WithMessage("Id is required.");
            RuleFor(a => a.Product.Name).NotEmpty().WithMessage("Name is required.");
            RuleFor(a => a.Product.ImageFile).NotEmpty().WithMessage("Image is required.");
            RuleFor(a => a.Product.Price).GreaterThan(0).WithMessage("Price must be greater than 0.");
            RuleFor(a => a.Product.Category).NotEmpty().WithMessage("Category is required.");
        }
    }
}
