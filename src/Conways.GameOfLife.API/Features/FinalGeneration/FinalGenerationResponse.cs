namespace Conways.GameOfLife.API.Features.FinalGeneration;

public record FinalGenerationResponse(string BoardId, bool Stable, bool[][] FinalGeneration);