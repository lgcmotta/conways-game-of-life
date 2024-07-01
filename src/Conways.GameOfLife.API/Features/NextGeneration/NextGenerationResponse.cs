namespace Conways.GameOfLife.API.Features.NextGeneration;

public record NextGenerationResponse(string BoardId, bool[][] Generation, bool Stable);