namespace Conways.GameOfLife.Domain.Exceptions;

public class BoardNotFoundException : Exception
{
    public BoardNotFoundException(string boardId)
        : base($"Board with Id '{boardId}' was not found")
    { }
}