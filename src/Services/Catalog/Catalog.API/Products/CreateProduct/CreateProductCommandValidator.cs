namespace Catalog.API.Products.CreateProduct
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(a => a.Name).NotEmpty().WithMessage("Name is required.");
            RuleFor(a => a.ImageFile).NotEmpty().WithMessage("Image is required.");
            RuleFor(a => a.Price).GreaterThan(0).WithMessage("Price must be greater than 0.");
            RuleFor(a => a.Category).NotEmpty().WithMessage("Category is required.");
        }
    }
}
