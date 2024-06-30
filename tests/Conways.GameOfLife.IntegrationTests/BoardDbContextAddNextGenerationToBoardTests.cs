namespace Conways.GameOfLife.IntegrationTests;

public class BoardDbContextAddNextGenerationToBoardTests : IClassFixture<ConwaysGameOfLifeWebApplicationFactory>, IAsyncLifetime
{
    private readonly ConwaysGameOfLifeWebApplicationFactory _factory;

    public BoardDbContextAddNextGenerationToBoardTests(ConwaysGameOfLifeWebApplicationFactory factory)
    {
        _factory = factory;
    }
    
    public async Task InitializeAsync()
    {
        await _factory.InitializeMongoDbContainerAsync();

        using var scope = _factory.Services.CreateScope();
        
        var context = scope.ServiceProvider.GetRequiredService<BoardDbContext>();
        
        await context.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _factory.DisposeAsync();
    }
    
    [Fact]
    public async Task AddNextGenerationToBoard_WhenSavingChangesToPostgresSQL_ShouldHaveBoardWithTwoGenerations()
    {
        // Arrange
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<BoardDbContext>();
            
            var initialState = new[,]
            {
                { true, true, false },
                { false, true, false },
                { false, false, false }
            };
            
            var board = new Board(initialState);
            
            await context.Set<Board>().AddAsync(board);
            
            await context.SaveChangesAsync();
            
        }

        // Act
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<BoardDbContext>();

            var board = context.Set<Board>()
                .Include("_generations")
                .FirstOrDefault(x => x.Id == 1);

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
                .FirstOrDefault(x => x.Id == 1);
            
            board.Should().NotBeNull();
            board!.Generations.Should().NotBeEmpty().And.HaveCount(2);
        }
    }
}