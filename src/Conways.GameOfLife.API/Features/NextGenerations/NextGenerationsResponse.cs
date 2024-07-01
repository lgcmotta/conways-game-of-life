namespace Conways.GameOfLife.API.Features.NextGenerations;

public record NextGenerationsResponse(string BoardId, bool Stable, IEnumerable<NextGenerationModel> Generations);