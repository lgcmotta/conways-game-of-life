using Conways.GameOfLife.Domain.Core;
using MediatR;

namespace Conways.GameOfLife.API.Features.CreateBoard;

public record CreateBoardCommand(bool[][] FirstGeneration) : IRequest<CreateBoardResponse>, ICommand;