namespace Conways.GameOfLife.Domain.UnitTests;

public class BoardTests
{
    public static TheoryData<bool[,]> GetInvalidArgumentsForBoardConstructor()
    {
        return new TheoryData<bool[,]>
        {
            new bool[0, 1],
            new bool[1, 0]
        };
    }

    [Theory]
    [MemberData(nameof(GetInvalidArgumentsForBoardConstructor))]
    public void Constructor_WhenConstructorArgumentsAreInvalid_ShouldThrowArgumentOutOfRangeException(bool[,] firstGeneration)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Board(firstGen: firstGeneration));
    }

    [Fact]
    public void AddGeneration_WhenNewGenerationsAdded_ShouldChangeCurrentGenerationAndIncrementCollection()
    {
        // Arrange
        var firstGeneration = new[,]
        {
            { true, false },
            { false, true }
        };
        
        var board = new Board(firstGen: firstGeneration);
        
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
    public void NextGeneration_WhenGivenAFirstGeneration_ShouldReturnValidNextState()
    {
        // Arrange
        var firstGeneration = new[,]
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

        var board = new Board(firstGen: firstGeneration);
        
        // Act
        bool[,] nextState = board.NextGeneration();
        
        // Assert
        nextState.Should().BeEquivalentTo(expectedNextState);
    }

    [Fact]
    public void HasReachedStableState_WhenStableStateReached_ShouldReturnTrue()
    {
        // Arrange
        var firstGeneration = new[,]
        {
            { true, true, false },
            { false, true, false },
            { false, false, false }
        };

        var board = new Board(firstGen: firstGeneration);
        
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

        var board = new Board(firstGen: unstableState);
        
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
        
        var firstGeneration = new[,]
        {
            { true, true, false },
            { false, true, false },
            { false, false, false }
        };
        
        var board = new Board(firstGen: firstGeneration);
        
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