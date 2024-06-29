namespace Conways.GameOfLife.Domain;

public sealed class BoardState
{
    private readonly bool[,] _value;

    public BoardState(bool[,] value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    public BoardState(int rows, int columns)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(rows);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(columns);
        
        _value = new bool[rows, columns];
    }
    
    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is BoardState other && Equals(other);
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }

    public static bool operator ==(BoardState? left, BoardState? right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }
        
        if (left.GetRows() != right.GetRows() || 
            left.GetColumns() != right.GetColumns())
        {
            return false;
        }
        
        for (var row = 0; row < left.GetRows(); row++)
        {
            for (var column = 0; column < left.GetColumns(); column++)
            {
                if (left._value[row, column] != right._value[row, column])
                {
                    return false;
                }
            }
        }

        return true;
    }
    
    public static bool operator !=(BoardState? left, BoardState? right) => !(left == right);
    
    public static implicit operator bool[,](BoardState state) => state._value;

    public static implicit operator BoardState(bool[,] value) => new(value);
    
    public bool this[int row, int column]
    {
        get => _value[row, column];
        set => _value[row, column] = value;
    }
    
    internal int GetRows() => _value.GetLength(0);

    internal int GetColumns() => _value.GetLength(1);
    
    internal int CountLiveNeighbors(int row, int column)
    {
        var rows = GetRows();
        var columns = GetColumns();

        var liveNeighbors = 0;

        for (var i = -1; i <= 1; i++)
        {
            for (var j = -1; j <= 1; j++)
            {
                if (i is 0 && j is 0)
                {
                    continue;
                }

                var neighborRow = row + i;
                var neighborColumn = column + j;

                if (neighborRow < 0 || neighborRow >= rows || neighborColumn < 0 || neighborColumn >= columns)
                {
                    continue;
                }
                
                if (_value[neighborRow, neighborColumn])
                {
                    liveNeighbors++;
                }
            }
        }

        return liveNeighbors;
    }

    internal bool HasReachedStableState(BoardState nextState)
    {
        return _value == nextState;
    }
    
    private bool Equals(BoardState other)
    {
        return _value.Equals(other._value);
    }
}