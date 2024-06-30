using Conways.GameOfLife.Domain.Core;
using MediatR;

namespace Conways.GameOfLife.API.Features.UploadBoard;

// ReSharper disable once ClassNeverInstantiated.Global
public record UploadBoardCommand(bool[][] FirstGeneration) : IRequest<UploadBoardResponse>, ICommand;