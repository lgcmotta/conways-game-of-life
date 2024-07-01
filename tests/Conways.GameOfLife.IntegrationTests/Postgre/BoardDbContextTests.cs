using Conways.GameOfLife.Infrastructure.PostgreSQL;

namespace Conways.GameOfLife.IntegrationTests.Postgre;

public class BoardDbContextTests : IClassFixture<ConwaysGameOfLifeWebApplicationFactory>
{
    private readonly ConwaysGameOfLifeWebApplicationFactory _factory;

    public BoardDbContextTests(ConwaysGameOfLifeWebApplicationFactory factory)
    {
        _factory = factory;
    }
    
    [Fact]
    public async Task AddAsync_WhenSavingChangesToPostgresSQL_ShouldHaveBoardWithGenerationPersisted()
    {
        // Arrange
        long boardId;
        
        var firstGeneration = new[,]
        {
            { true, true, false },
            { false, true, false },
            { false, false, false }
        };

        // Act
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<BoardDbContext>();

            var board = new Board(firstGeneration);
            
            await context.Set<Board>().AddAsync(board);
            
            await context.SaveChangesAsync();

            boardId = board.Id;

        }

        // Assert
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<BoardDbContext>();
            
            var board = context.Set<Board>()
                .Include("_generations")
                .FirstOrDefault(x => x.Id == boardId);

            board.Should().NotBeNull();
            board!.Generations.Should().NotBeEmpty();
        }
    }
    
    [Fact]
    public async Task AddNextGenerationToBoard_WhenSavingChangesToPostgresSQL_ShouldHaveBoardWithTwoGenerations()
    {
        // Arrange
        long boardId;
        
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<BoardDbContext>();
            
            var firstGeneration = new[,]
            {
                { true, true, false },
                { false, true, false },
                { false, false, false }
            };
            
            var board = new Board(firstGeneration);
            
            await context.Set<Board>().AddAsync(board);
            
            await context.SaveChangesAsync();

            boardId = board.Id;
        }

        // Act
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<BoardDbContext>();

            var board = context.Set<Board>()
                .Include("_generations")
                .FirstOrDefault(x => x.Id == boardId);

            var nextGen = board!.NextGeneration();
            
            board.AddGeneration(1, nextGen);
            
            await context.SaveChangesAsync();
            
        }

        // Assert
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<BoardDbContext>();
            
            var board = context.Set<Board>()
                .Include("_generations")
                .FirstOrDefault(x => x.Id == boardId);
            
            board.Should().NotBeNull();
            board!.Generations.Should().NotBeEmpty().And.HaveCount(2);
        }
    }
}