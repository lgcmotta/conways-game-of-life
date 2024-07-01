using MediatR;

namespace Conways.GameOfLife.API.Features.NextGeneration;

// ReSharper disable once ClassNeverInstantiated.Global
public record NextGenerationQuery(string BoardId) : IRequest<NextGenerationResponse>;