using FluentValidation;

namespace Conways.GameOfLife.API.Features.FinalGeneration;

public class FinalGenerationQueryValidator : AbstractValidator<FinalGenerationQuery>
{
    public FinalGenerationQueryValidator()
    {
        RuleFor(query => query.BoardId)
            .NotEmpty()
            .NotNull();

        RuleFor(query => query.MaxAttempts)
            .GreaterThanOrEqualTo(1);
    }
}