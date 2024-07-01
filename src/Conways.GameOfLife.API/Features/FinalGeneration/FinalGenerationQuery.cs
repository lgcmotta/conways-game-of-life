using Conways.GameOfLife.Domain.Core;
using MediatR;

namespace Conways.GameOfLife.API.Features.FinalGeneration;

public record FinalGenerationQuery(string BoardId, int MaxAttempts) : IRequest<FinalGenerationResponse>, IQuery;