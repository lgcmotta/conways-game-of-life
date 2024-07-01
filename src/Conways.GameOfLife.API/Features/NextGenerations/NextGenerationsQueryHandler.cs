using Conways.GameOfLife.Domain;
using Conways.GameOfLife.Domain.Exceptions;
using Conways.GameOfLife.Infrastructure.Extensions;
using Conways.GameOfLife.Infrastructure.PostgresSQL;
using HashidsNet;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Conways.GameOfLife.API.Features.NextGenerations;

public class NextGenerationsQueryHandler : IRequestHandler<NextGenerationsQuery, NextGenerationsResponse>
{
    private readonly BoardDbContext _context;
    private readonly IHashids _hashids;

    public NextGenerationsQueryHandler(BoardDbContext context, IHashids hashids)
    {
        _context = context;
        _hashids = hashids;
    }
    
    public async Task<NextGenerationsResponse> Handle(NextGenerationsQuery request, CancellationToken cancellationToken)
    {
        var boardId = _hashids.DecodeLong(request.BoardId)[0];

        var board = await _context.Set<Board>()
            .Include("_generations")
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == boardId, cancellationToken)
            .ConfigureAwait(continueOnCapturedContext: false);

        if (board is null)
        {
            throw new BoardNotFoundException(request.BoardId);
        }

        var stable = false;
        
        foreach (var generation in Enumerable.Range(0, request.Generations))
        {
            var nextGeneration = board.NextGeneration();

            stable = board.HasReachedStableState(nextGeneration);
            
            board.AddGeneration(generation + 1, nextGeneration);
        }
        
        return new NextGenerationsResponse(
            request.BoardId, 
            stable,
            board.Generations.Select(gen => new NextGenerationModel(gen.Number, gen.ToMatrix()))
        );
    }
}