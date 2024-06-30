namespace Conways.GameOfLife.Domain.UnitTests;

public class BoardTests
{
    public static IEnumerable<object[]> GetInvalidArgumentsForBoardConstructor()
    {
        yield return [new bool[0, 1]];
        yield return [new bool[1, 0]];
    }

    [Theory]
    [MemberData(nameof(GetInvalidArgumentsForBoardConstructor))]
    public void Constructor_WhenConstructorArgumentsAreInvalid_ShouldThrowArgumentOutOfRangeException(bool[,] initialState)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Board(initialState: initialState));
    }

    [Fact]
    public void AddGeneration_WhenNewGenerationsAdded_ShouldChangeCurrentGenerationAndIncrementCollection()
    {
        // Arrange
        var initialState = new[,]
        {
            { true, false },
            { false, true }
        };
        
        var board = new Board(initialState: initialState);
        
        Generation newGeneration = new[,]
        {
            { false, true },
            { true, false }
        };

        // Act
        board.AddGeneration(1, newGeneration);
        
        // Assert
        board.CurrentGeneration.Should().BeEquivalentTo(newGeneration);
        board.Generations.Should().NotBeEmpty().And.HaveCount(2);
    }

    [Fact]
    public void NextGeneration_WhenGivenAInitialState_ShouldReturnValidNextState()
    {
        // Arrange
        var initialState = new[,]
        {
            { true, true, false },
            { false, true, false },
            { false, false, false }
        };
        
        var expectedNextState = new [,]
        {
            { true, true, false },
            { true, true, false },
            { false, false, false }
        };

        var board = new Board(initialState: initialState);
        
        // Act
        bool[,] nextState = board.NextGeneration();
        
        // Assert
        nextState.Should().BeEquivalentTo(expectedNextState);
    }

    [Fact]
    public void HasReachedStableState_WhenStableStateReached_ShouldReturnTrue()
    {
        // Arrange
        var initialState = new[,]
        {
            { true, true, false },
            { false, true, false },
            { false, false, false }
        };

        var board = new Board(initialState: initialState);
        
        var expectedNextState = new [,]
        {
            { true, true, false },
            { true, true, false },
            { false, false, false }
        };

        // Act
        board.AddGeneration(1, expectedNextState);
        
        bool[,] currentState = board.CurrentGeneration;
        
        // Assert
        currentState.Should().BeEquivalentTo(expectedNextState);
    }

    [Fact]
    public void HasReachedStableState_WhenNotStableState_ShouldReturnFalse()
    {
        // Arrange
        var unstableState = new[,]
        {
            { false, true, false },
            { true, true, false },
            { false, false, false }
        };

        var board = new Board(initialState: unstableState);
        
        // Act
        var stable = board.HasReachedStableState();
        
        // Assert
        stable.Should().BeFalse();
    }
    
    [Theory]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(8)]
    [InlineData(16)]
    [InlineData(32)]
    public void BoardGenerationSimulation_WhenReachingStableState_ShouldKeepStableAfterManyIterations(int iterations)
    {
        // Arrange
        var stable = false;
        
        var initialState = new[,]
        {
            { true, true, false },
            { false, true, false },
            { false, false, false }
        };
        
        var board = new Board(initialState: initialState);
        
        var expectedNextState = new [,]
        {
            { true, true, false },
            { true, true, false },
            { false, false, false }
        };

        // Act
        foreach (var iteration in Enumerable.Range(0, iterations))
        {
            var nextState = board.NextGeneration();
            
            board.AddGeneration(iteration + 1, nextState);
            
            stable = board.HasReachedStableState(nextState);
        }

        bool[,] currentState = board.CurrentGeneration;

        // Assert
        stable.Should().BeTrue();
        currentState.Should().BeEquivalentTo(expectedNextState);
    }
}