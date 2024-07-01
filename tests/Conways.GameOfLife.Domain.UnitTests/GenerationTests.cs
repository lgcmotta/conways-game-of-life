namespace Conways.GameOfLife.Domain.UnitTests;

public class GenerationTests
{
    public static TheoryData<Generation, Generation, bool> GetBoardGenerationsForEqualityOperatorGuardClauses()
    {
        var referenceGeneration = new Generation(new[,]
        {
            { true, true, false }, { false, false, false }, { false, false, false }
        });

        return new TheoryData<Generation, Generation, bool>
        {
            { referenceGeneration, referenceGeneration, true },
            { referenceGeneration, referenceGeneration, true },
            { null!, referenceGeneration, false },
            { referenceGeneration, null!, false },
            { null!, null!, true },
            {
                new[,] { { false, false, true }, { true, true, false } },
                new[,] { { false, false, true }, { true, true, false }, { false, true, false } },
                false
            },
            {
                new[,] { { false, false }, { true, true } },
                new[,] { { false, false, true }, { true, true, false } },
                false
            }
        };

    }

    public static TheoryData<bool[,], (int, int), int> GetBoardGenerationsForCountLiveNeighbors()
    {
        return new TheoryData<bool[,], (int, int), int>
        {
            { new[,] { { true, true, false }, { false, true, false }, { false, false, false } }, (0, 0), 2 },
            { new[,] { { false, false, false }, { false, true, false }, { false, false, false } }, (1, 1), 0 },
            { new[,] { { false, false, false }, { false, true, false }, { true, false, false } }, (2, 0), 1 }
        };
    }
    
    [Fact]
    public void Constructor_WhenFirstGenerationIsNull_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new Generation(null!));
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0, 1)]
    [InlineData(1, 0)]
    public void Constructor_WhenRowsOrColumnsAreZeroOrNegative_ShouldThrowArgumentOutOfRangeException(int rows, int columns)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Generation(rows, columns));
    }

    [Fact]
    public void Constructor_WhenGivenAValidNumberOfRowsAndColumns_ShouldInitializeADeadState()
    {
        // Arrange
        const int rows = 2;
        const int columns = 2;
        
        // Act
        var generation = new Generation(rows, columns);

        // Assert
        generation[0, 0].Should().BeFalse();
        generation[0, 1].Should().BeFalse();
        generation[1, 0].Should().BeFalse();
        generation[1, 1].Should().BeFalse();
    }
    
    [Theory]
    [MemberData(nameof(GetBoardGenerationsForEqualityOperatorGuardClauses))]
    public void EqualityOperator_WhenComparingGenerations_ShouldValidateGuardClauses(Generation? left, Generation? right, bool expected)
    {
        // Act
        var equal = left == right;

        // Assert
        equal.Should().Be(expected);
    }

    [Fact]
    public void EqualityOperator_WhenGenerationsAreEqual_ShouldReturnTrue()
    {
        // Arrange
        var firstGeneration1 = new[,] { { true, false }, { false, true } };
        var firstGeneration2 = new[,] { { true, false }, { false, true } };

        var generation1 = new Generation(firstGeneration1);
        var generation2 = new Generation(firstGeneration2);

        // Act
        var actual = generation1 == generation2;

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void EqualityOperator_WhenGenerationsAreNotEqual_ShouldReturnFalse()
    {
        // Arrange
        var firstGeneration1 = new[,] { { true, false }, { false, true } };
        var firstGeneration2 = new[,] { { false, true }, { true, false } };

        var generation1 = new Generation(firstGeneration1);
        var generation2 = new Generation(firstGeneration2);

        // Act
        var actual = generation1 == generation2;

        // Assert
        actual.Should().BeFalse();
    }

    [Fact]
    public void InequalityOperator_WhenGenerationsAreEqual_ShouldReturnFalse()
    {
        // Arrange
        var firstGeneration1 = new[,] { { true, false }, { false, true } };
        var firstGeneration2 = new[,] { { true, false }, { false, true } };

        var generation1 = new Generation(firstGeneration1);
        var generation2 = new Generation(firstGeneration2);
        
        // Act
        var equal = generation1 != generation2;

        // Assert
        equal.Should().BeFalse();
    }
    
    [Fact]
    public void InequalityOperator_WhenGenerationsAreNotEqual_ShouldReturnTrue()
    {
        // Arrange
        var firstGeneration1 = new[,] { { true, false }, { false, true } };
        var firstGeneration2 = new[,] { { false, true }, { true, false } };

        var generation1 = new Generation(firstGeneration1);
        var generation2 = new Generation(firstGeneration2);
        
        // Act
        var equal = generation1 != generation2;

        // Assert
        equal.Should().BeTrue();
    }
    
    [Fact]
    public void ImplicitOperator_WhenConvertingFromGeneration_ShouldReturn2DArrayOfBooleans()
    {
        // Arrange
        var generation = new Generation(new [,] { { false, false }, { false, false } });
        
        // Act
        bool[,] value = generation;

        // Assert
        value.Should().BeOfType<bool[,]>();
    }

    [Theory]
    [MemberData(nameof(GetBoardGenerationsForCountLiveNeighbors))]
    public void CountLiveNeighbors_WhenBoardIsValid_ShouldReturnCorrectCount(bool[,] firstGeneration, (int, int) coordinates, int expected)
    {
        // Arrange
        var generation = new Generation(firstGeneration);
        var (row, column) = coordinates;

        // Act
        var liveNeighbors = generation.CountLiveNeighbors(row, column);

        // Assert
        liveNeighbors.Should().Be(expected);
    }

    [Fact]
    public void HasReachedStableState_WhenNextStateIsSameAsCurrentState_ShouldReturnTrue()
    {
        // Arrange
        var firstGeneration = new[,]
        {
            { true, true, false },
            { false, true, false },
            { false, false, false }
        };

        var nextGeneration = new[,]
        {
            { true, true, false },
            { false, true, false },
            { false, false, false }
        };

        var generation1 = new Generation(firstGeneration);
        var generation2 = new Generation(nextGeneration);

        // Act
        var stableState = generation1.HasReachedStableState(generation2);

        // Assert
        stableState.Should().BeTrue();
    }

    [Fact]
    public void HasReachedStableState_WhenNextStateIsNotTheSameAsCurrentState_ShouldReturnFalse()
    {
        // Arrange
        var firstGeneration = new[,]
        {
            { false, false, false },
            { true, true, false },
            { true, false, false }
        };

        var nextGeneration = new[,]
        {
            { true, true, false },
            { false, true, false },
            { false, false, false }
        };

        var generation1 = new Generation(firstGeneration);
        var generation2 = new Generation(nextGeneration);

        // Act
        var stableState = generation1.HasReachedStableState(generation2);

        // Assert
        stableState.Should().BeFalse();
    }
}