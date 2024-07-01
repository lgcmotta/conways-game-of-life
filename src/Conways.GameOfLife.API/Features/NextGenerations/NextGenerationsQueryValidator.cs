using FluentValidation;

namespace Conways.GameOfLife.API.Features.NextGenerations;

public class NextGenerationsQueryValidator : AbstractValidator<NextGenerationsQuery>
{
    public NextGenerationsQueryValidator()
    {
        RuleFor(query => query.BoardId)
            .NotEmpty()
            .NotNull();

        RuleFor(query => query.Generations)
            .GreaterThanOrEqualTo(1);
    }
}