using MediatR;

namespace Conways.GameOfLife.API.Features.NextGeneration;

public record NextGenerationQuery(string BoardId) : IRequest<NextGenerationResponse>;