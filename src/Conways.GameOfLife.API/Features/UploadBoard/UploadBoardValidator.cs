using FluentValidation;

namespace Conways.GameOfLife.API.Features.UploadBoard;

public class UploadBoardValidator : AbstractValidator<UploadBoardCommand>
{
    public UploadBoardValidator()
    {
        RuleFor(command => command.FirstGeneration)
            .NotNull()
            .WithMessage("must not be null")
            .NotEmpty()
            .WithMessage("must not be empty");

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        When(command => command.FirstGeneration is not null, () =>
        {
            RuleFor(command => command.FirstGeneration)
                .Must(rows => rows!.All(col => col.Length == rows!.Length))
                .WithMessage("must be a square matrix")
                .Must(rows => rows.Length >= 3 && rows.All(col => col.Length >= 3))
                .WithMessage("must be at least a 3x3 matrix");
        });
    }
}