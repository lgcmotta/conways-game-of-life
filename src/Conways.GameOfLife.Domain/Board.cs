namespace Conways.GameOfLife.Domain;

public sealed class Board
{
    public Board(int id, bool[,] initialState)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        if (initialState.GetLength(dimension: 0) <= 0 || initialState.GetLength(dimension: 1) <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(initialState));
        } 

        Id = id;
        InitialState = CurrentState = new BoardState(initialState);
    }

    public int Id { get; private set; }

    public BoardState CurrentState { get; private set; }

    public BoardState InitialState { get; private set; }
    
    public void UpdateCurrentState(BoardState nextState) => CurrentState = nextState;
    
    public BoardState ProjectNextState()
    {
        var nextState = GenerateNextState();
        
        return nextState;
    }
    
    public bool HasReachedStableState(BoardState? nextState = null)
    {
        nextState ??= GenerateNextState();

        return CurrentState.HasReachedStableState(nextState);
    }
    
    private BoardState GenerateNextState()
    {
        var rows = CurrentState.GetRows();
        
        var columns = CurrentState.GetColumns();

        var nextState = new BoardState(rows, columns);

        for (var row = 0; row < rows; row++)
        {
            for (var column = 0; column < columns; column++)
            {
                var liveNeighbors = CurrentState.CountLiveNeighbors(row, column);
                
                if (CurrentState[row, column])
                {
                    nextState[row, column] = liveNeighbors is 2 or 3;
                }
                else
                {
                    nextState[row, column] = liveNeighbors is 3;
                }
            }
        }

        return nextState;
    }
}