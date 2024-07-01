using FluentValidation;

namespace Conways.GameOfLife.API.Features.NextGeneration;

public class NextGenerationQueryValidator : AbstractValidator<NextGenerationQuery>
{
    public NextGenerationQueryValidator()
    {
        RuleFor(query => query.BoardId)
            .NotEmpty()
            .NotNull();
    }
}