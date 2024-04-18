using MediatR.Pipeline;
using ValidationException = Application.Common.Exceptions.ValidationException;

namespace Application.Common.PreProcessors;

public class ValidationRequestPreProcessor<TRequest>(IEnumerable<IValidator<TRequest>> validators)
    : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        if (validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                validators.Select(v =>
                    v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .Where(r => r.Errors.Any())
                .SelectMany(r => r.Errors)
                .ToList();

            if (failures.Count != 0)
            {
                throw new ValidationException(failures);
            }
        }
    }
}