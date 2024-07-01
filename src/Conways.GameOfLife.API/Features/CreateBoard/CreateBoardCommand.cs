using Conways.GameOfLife.Domain.Core;
using MediatR;

namespace Conways.GameOfLife.API.Features.CreateBoard;

// ReSharper disable once ClassNeverInstantiated.Global
public record CreateBoardCommand(bool[][] FirstGeneration) : IRequest<CreateBoardResponse>, ICommand;