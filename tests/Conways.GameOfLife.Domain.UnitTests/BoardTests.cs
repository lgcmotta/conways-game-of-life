namespace Conways.GameOfLife.Domain.UnitTests;

public class BoardTests
{
    public static IEnumerable<object[]> GetInvalidArgumentsForBoardConstructor()
    {
        yield return [0, new bool[1, 1]];
        yield return [-1, new bool[1, 1]];
        yield return [1, new bool[0, 1]];
        yield return [1, new bool[1, 0]];
    }

    [Theory]
    [MemberData(nameof(GetInvalidArgumentsForBoardConstructor))]
    public void Constructor_WhenConstructorArgumentsAreInvalid_ShouldThrowArgumentOutOfRangeException(int id, bool[,] initialState)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Board(id: id, initialState: initialState));
    }

    [Fact]
    public void UpdateCurrentState_WhenCalled_ShouldUpdateCurrentState()
    {
        // Arrange
        var initialState = new[,]
        {
            { true, false },
            { false, true }
        };
        
        var board = new Board(id: 1, initialState: initialState);
        
        BoardState newState = new[,]
        {
            { false, true },
            { true, false }
        };

        // Act
        board.UpdateCurrentState(newState);
        
        // Assert
        board.CurrentState.Should().BeEquivalentTo(newState);
    }

    [Fact]
    public void ProjectNextState_WhenGivenAInitialState_ShouldReturnValidNextState()
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

        var board = new Board(id: 1, initialState: initialState);
        
        // Act
        bool[,] nextState = board.ProjectNextState();
        
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

        var board = new Board(id: 1, initialState: initialState);
        
        var expectedNextState = new [,]
        {
            { true, true, false },
            { true, true, false },
            { false, false, false }
        };

        // Act
        board.UpdateCurrentState(expectedNextState);
        
        bool[,] currentState = board.CurrentState;
        
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

        var board = new Board(id: 1, initialState: unstableState);
        
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
    public void BoardProjection_WhenReachingStableState_ShouldKeepStableAfterManyIterations(int iterations)
    {
        // Arrange
        var stable = false;
        
        var initialState = new[,]
        {
            { true, true, false },
            { false, true, false },
            { false, false, false }
        };
        
        var board = new Board(id: 1, initialState: initialState);
        
        var expectedNextState = new [,]
        {
            { true, true, false },
            { true, true, false },
            { false, false, false }
        };

        // Act
        foreach (var _ in Enumerable.Range(0, iterations))
        {
            var nextState = board.ProjectNextState();
            
            board.UpdateCurrentState(nextState);
            
            stable = board.HasReachedStableState(nextState);
        }

        bool[,] currentState = board.CurrentState;

        // Assert
        stable.Should().BeTrue();
        currentState.Should().BeEquivalentTo(expectedNextState);
    }
}