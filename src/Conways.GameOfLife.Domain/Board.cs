using Conways.GameOfLife.Domain.Core;

namespace Conways.GameOfLife.Domain;

public sealed class Board : IAggregateRoot, IEntity
{
    private readonly List<Generation> _generations = [];
    
    public Board(bool[,] firstGen) : this()
    {
        if (firstGen.GetLength(dimension: 0) <= 0 || firstGen.GetLength(dimension: 1) <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(firstGen));
        } 
        
        var firstGeneration = new Generation(firstGen);
        firstGeneration.DefineStateGeneration(0);
        _generations.Add(firstGeneration);
    }
    
    private Board()
    { }

    public long Id { get; private set; }

    public Generation CurrentGeneration => _generations.MaxBy(x => x.Number) ?? _generations[0];

    public IReadOnlyCollection<Generation> Generations => _generations.AsReadOnly();
    
    public void AddGeneration(long generationNumber, bool[,] state)
    {
        if (_generations.Any(g => g.Number == generationNumber))
        {
            throw new ArgumentOutOfRangeException(nameof(generationNumber));
        }

        var generation = new Generation(state);
        
        generation.DefineStateGeneration(generationNumber);
        
        _generations.Add(generation);
    }

    public Generation NextGeneration()
    {
        var nextGeneration = CalculateNextGeneration();
        
        return nextGeneration;
    }
    
    public bool HasReachedStableState(Generation? nextGeneration = null)
    {
        nextGeneration ??= CalculateNextGeneration();

        var stable= CurrentGeneration.HasReachedStableState(nextGeneration);

        if (stable)
        {
            CurrentGeneration.StabilizeGeneration();
        }
        
        return stable;
    }
    
    private Generation CalculateNextGeneration()
    {
        var rows = CurrentGeneration.GetRows();
        
        var columns = CurrentGeneration.GetColumns();

        var nextGeneration = new Generation(rows, columns);

        for (var row = 0; row < rows; row++)
        {
            for (var column = 0; column < columns; column++)
            {
                var liveNeighbors = CurrentGeneration.CountLiveNeighbors(row, column);
                
                if (CurrentGeneration[row, column])
                {
                    nextGeneration[row, column] = liveNeighbors is 2 or 3;
                }
                else
                {
                    nextGeneration[row, column] = liveNeighbors is 3;
                }
            }
        }

        return nextGeneration;
    }
}