using BuildingBlocks.CQRS;
using FluentValidation;
using MediatR;

namespace BuildingBlocks.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse>
        (IEnumerable<IValidator<TRequest>> validators) 
        : IPipelineBehavior<TRequest, TResponse> 
        where TRequest : ICommand<TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var validationContext = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(validators.Select(a => a.ValidateAsync(validationContext, cancellationToken)));

            var faillures = validationResults.Where(a => a.Errors.Any()).SelectMany(a => a.Errors);

            if (faillures.Any())
            {
                throw new ValidationException(faillures);
            }

            return await next(cancellationToken);
        }
    }
}
