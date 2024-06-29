namespace Conways.GameOfLife.Domain.UnitTests;

public class BoardStateTests
{
    [Fact]
    public void Constructor_WhenInitialStateIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BoardState(null!));
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0, 1)]
    [InlineData(1, 0)]
    public void Constructor_WhenRowsOrColumnsAreZeroOrNegative_ShouldThrowArgumentOutOfRangeException(int rows, int columns)
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new BoardState(rows, columns));
    }
    
    [Fact]
    public void EqualityOperator_WhenStatesAreEqual_ShouldReturnTrue()
    {
        // Arrange
        var initialState1 = new[,] { { true, false }, { false, true } };
        var initialState2 = new[,] { { true, false }, { false, true } };
        
        var boardState1 = new BoardState(initialState1);
        var boardState2 = new BoardState(initialState2);
        
        // Act
        var actual = boardState1 == boardState2;
        
        // Assert
        actual.Should().BeTrue();
    }
    
    [Fact]
    public void EqualityOperator_WhenStatesAreNotEqual_ShouldReturnFalse()
    {
        // Arrange
        var initialState1 = new[,] { { true, false }, { false, true } };
        var initialState2 = new[,] { { false, true }, { true, false } };
        
        var boardState1 = new BoardState(initialState1);
        var boardState2 = new BoardState(initialState2);
        
        // Act
        var actual = boardState1 == boardState2;
        
        // Assert
        actual.Should().BeFalse();
    }

    public static IEnumerable<object[]> GetBoardStateForCountLiveNeighbors()
    {
        yield return [new[,] { { true, true, false }, { false, true, false }, { false, false, false } }, (0, 0), 2];
        yield return [new[,] { { false, false, false }, { false, true, false }, { false, false, false } }, (1, 1), 0];
        yield return [new[,] { { false, false, false }, { false, true, false }, { true, false, false } }, (2, 0) , 1];
    }
    
    [Theory]
    [MemberData(nameof(GetBoardStateForCountLiveNeighbors))]
    public void CountLiveNeighbors_WhenBoardIsValid_ShouldReturnCorrectCount(bool[,] state, (int, int) coordinates ,int expected)
    {
        // Arrange
        var boardState = new BoardState(state);
        var (row, column) = coordinates;

        // Act
        var liveNeighbors = boardState.CountLiveNeighbors(row, column);
        
        // Assert
        liveNeighbors.Should().Be(expected);
    }

    [Fact]
    public void HasReachedStableState_WhenNextStateIsSameAsCurrentState_ShouldReturnTrue()
    {
        // Arrange
        var initialState = new[,]
        {
            { true, true, false },
            { false, true, false },
            { false, false, false }
        };
        
        var nextState = new [,]
        {
            { true, true, false },
            { false, true, false },
            { false, false, false }
        };

        var boardState1 = new BoardState(initialState);
        var boardState2 = new BoardState(nextState);

        // Act
        var stableState = boardState1.HasReachedStableState(boardState2);

        // Assert
        stableState.Should().BeTrue();
    }
    
    [Fact]
    public void HasReachedStableState_WhenNextStateIsNotTheSameAsCurrentState_ShouldReturnFalse()
    {
        // Arrange
        var initialState = new[,]
        {
            { false, false, false },
            { true, true, false },
            { true, false, false }
        };
        
        var nextState = new [,]
        {
            { true, true, false },
            { false, true, false },
            { false, false, false }
        };

        var boardState1 = new BoardState(initialState);
        var boardState2 = new BoardState(nextState);

        // Act
        var stableState = boardState1.HasReachedStableState(boardState2);

        // Assert
        stableState.Should().BeFalse();
    }
}