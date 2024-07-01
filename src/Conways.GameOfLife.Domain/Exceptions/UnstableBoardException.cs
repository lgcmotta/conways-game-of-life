namespace Conways.GameOfLife.Domain.Exceptions;

public class UnstableBoardException : Exception
{
    public UnstableBoardException(string boardId, int maxAttempts)
        : base($"Board with id '{boardId}' failed to reach stable state after {maxAttempts} attempts")
    { }
}