namespace Conways.GameOfLife.Domain.UnitTests;

public class BoardStateTests
{
    public static IEnumerable<object[]> GetBoardStateForEqualityOperatorGuardClauses()
    {
        var referenceBoardState = new BoardState(new[,]
        {
            { true, true, false }, { false, false, false }, { false, false, false }
        });
        
        yield return [referenceBoardState, referenceBoardState, true];
        yield return [null!, referenceBoardState, false];
        yield return [referenceBoardState, null!, false];
        yield return [null!, null!, true];
        yield return
        [
            new[,] { { false, false, true }, { true, true, false } },
            new[,] { { false, false, true }, { true, true, false }, { false, true, false } },
            false
        ];
        yield return
        [
            new[,] { { false, false }, { true, true } },
            new[,] { { false, false, true }, { true, true, false } },
            false
        ];
    }
    
    public static IEnumerable<object[]> GetBoardStateForCountLiveNeighbors()
    {
        yield return [new[,] { { true, true, false }, { false, true, false }, { false, false, false } }, (0, 0), 2];
        yield return [new[,] { { false, false, false }, { false, true, false }, { false, false, false } }, (1, 1), 0];
        yield return [new[,] { { false, false, false }, { false, true, false }, { true, false, false } }, (2, 0), 1];
    }
    
    [Fact]
    public void Constructor_WhenInitialStateIsNull_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new BoardState(null!));
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0, 1)]
    [InlineData(1, 0)]
    public void Constructor_WhenRowsOrColumnsAreZeroOrNegative_ShouldThrowArgumentOutOfRangeException(int rows, int columns)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new BoardState(rows, columns));
    }

    [Fact]
    public void Constructor_WhenGivenAValidNumberOfRowsAndColumns_ShouldInitializeADeadState()
    {
        // Arrange
        const int rows = 2;
        const int columns = 2;
        
        // Act
        var boardState = new BoardState(rows, columns);

        // Assert
        boardState[0, 0].Should().BeFalse();
        boardState[0, 1].Should().BeFalse();
        boardState[1, 0].Should().BeFalse();
        boardState[1, 1].Should().BeFalse();
    }
    
    [Theory]
    [MemberData(nameof(GetBoardStateForEqualityOperatorGuardClauses))]
    public void EqualityOperator_WhenComparingStates_ShouldValidateGuardClauses(BoardState? left, BoardState? right, bool expected)
    {
        // Act
        var equal = left == right;

        // Assert
        equal.Should().Be(expected);
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

    [Fact]
    public void InequalityOperator_WhenStatesAreEqual_ShouldReturnFalse()
    {
        // Arrange
        var initialState1 = new[,] { { true, false }, { false, true } };
        var initialState2 = new[,] { { true, false }, { false, true } };

        var boardState1 = new BoardState(initialState1);
        var boardState2 = new BoardState(initialState2);
        
        // Act
        var equal = boardState1 != boardState2;

        // Assert
        equal.Should().BeFalse();
    }
    
    [Fact]
    public void InequalityOperator_WhenStatesAreNotEqual_ShouldReturnTrue()
    {
        // Arrange
        var initialState1 = new[,] { { true, false }, { false, true } };
        var initialState2 = new[,] { { false, true }, { true, false } };

        var boardState1 = new BoardState(initialState1);
        var boardState2 = new BoardState(initialState2);
        
        // Act
        var equal = boardState1 != boardState2;

        // Assert
        equal.Should().BeTrue();
    }
    
    [Fact]
    public void ImplicitOperator_WhenConvertingFromBoardState_ShouldReturn2DArrayOfBooleans()
    {
        // Arrange
        var boardState = new BoardState(new [,] { { false, false }, { false, false } });
        
        // Act
        bool[,] value = boardState;

        // Assert
        value.Should().BeOfType<bool[,]>();
    }

    [Theory]
    [MemberData(nameof(GetBoardStateForCountLiveNeighbors))]
    public void CountLiveNeighbors_WhenBoardIsValid_ShouldReturnCorrectCount(bool[,] state, (int, int) coordinates, int expected)
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

        var nextState = new[,]
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

        var nextState = new[,]
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