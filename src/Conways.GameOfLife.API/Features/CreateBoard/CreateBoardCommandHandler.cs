using Conways.GameOfLife.Domain;
using Conways.GameOfLife.Infrastructure.Extensions;
using Conways.GameOfLife.Infrastructure.PostgreSQL;
using HashidsNet;
using MediatR;

namespace Conways.GameOfLife.API.Features.CreateBoard;

public class CreateBoardCommandHandler : IRequestHandler<CreateBoardCommand, CreateBoardResponse>
{
    private readonly BoardDbContext _context;
    private readonly IHashids _hashids;

    public CreateBoardCommandHandler(BoardDbContext context, IHashids hashids)
    {
        _context = context;
        _hashids = hashids;
    }

    public async Task<CreateBoardResponse> Handle(CreateBoardCommand request, CancellationToken cancellationToken)
    {
        var board = new Board(request.FirstGeneration.ToMultiArray());

        await _context.Set<Board>().AddAsync(board, cancellationToken)
            .ConfigureAwait(continueOnCapturedContext: false);

        await _context.SaveChangesAsync(cancellationToken)
            .ConfigureAwait(continueOnCapturedContext: false);

        var boardId = _hashids.EncodeLong(board.Id);
        
        return new CreateBoardResponse(boardId);
    }
}