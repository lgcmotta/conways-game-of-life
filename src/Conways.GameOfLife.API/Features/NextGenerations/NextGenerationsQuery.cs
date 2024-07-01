using Conways.GameOfLife.Domain.Core;
using MediatR;

namespace Conways.GameOfLife.API.Features.NextGenerations;

public record NextGenerationsQuery(string BoardId, int Generations) : IRequest<NextGenerationsResponse>, IQuery;