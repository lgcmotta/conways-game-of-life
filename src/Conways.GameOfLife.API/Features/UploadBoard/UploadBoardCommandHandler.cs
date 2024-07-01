using Conways.GameOfLife.Domain;
using Conways.GameOfLife.Infrastructure.Extensions;
using Conways.GameOfLife.Infrastructure.PostgreSQL;
using HashidsNet;
using MediatR;

namespace Conways.GameOfLife.API.Features.UploadBoard;

public class UploadBoardCommandHandler : IRequestHandler<UploadBoardCommand, UploadBoardResponse>
{
    private readonly BoardDbContext _context;
    private readonly IHashids _hashids;

    public UploadBoardCommandHandler(BoardDbContext context, IHashids hashids)
    {
        _context = context;
        _hashids = hashids;
    }

    public async Task<UploadBoardResponse> Handle(UploadBoardCommand request, CancellationToken cancellationToken)
    {
        var board = new Board(request.FirstGeneration!.ToMultiArray());

        await _context.Set<Board>().AddAsync(board, cancellationToken)
            .ConfigureAwait(continueOnCapturedContext: false);

        await _context.SaveChangesAsync(cancellationToken)
            .ConfigureAwait(continueOnCapturedContext: false);

        var boardId = _hashids.EncodeLong(board.Id);
        
        return new UploadBoardResponse(boardId);
    }
}