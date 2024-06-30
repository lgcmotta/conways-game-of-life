namespace Conways.GameOfLife.Domain;

public sealed class Generation
{
    private readonly bool[,] _value;
    
    public Generation(bool[,] value) : this()
    {
        ArgumentNullException.ThrowIfNull(value);
        
        _value = value;
    }
    
    private Generation()
    { }

    public Generation(int rows, int columns)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(rows);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(columns);
        
        _value = new bool[rows, columns];
    }

    public long Number { get; private set; }
    
    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is Generation other && Equals(other);
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }

    public static bool operator ==(Generation? left, Generation? right)
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
    
    public static bool operator !=(Generation? left, Generation? right) => !(left == right);
    
    public static implicit operator bool[,](Generation generation) => generation._value;

    public static implicit operator Generation(bool[,] value) => new(value);
    
    public bool this[int row, int column]
    {
        get => _value[row, column];
        set => _value[row, column] = value;
    }

    public int GetRows() => _value.GetLength(0);

    public int GetColumns() => _value.GetLength(1);

    internal void DefineStateGeneration(long generation) => Number = generation;
    
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

    internal bool HasReachedStableState(Generation nextGeneration)
    {
        return this == nextGeneration;
    }
    
    private bool Equals(Generation other)
    {
        return _value.Equals(other._value);
    }
}