namespace Conways.GameOfLife.IntegrationTests;

public class BoardDbContextAddBoardTests : IClassFixture<ConwaysGameOfLifeWebApplicationFactory>, IAsyncLifetime
{
    private readonly ConwaysGameOfLifeWebApplicationFactory _factory;

    public BoardDbContextAddBoardTests(ConwaysGameOfLifeWebApplicationFactory factory)
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
    public async Task AddAsync_WhenSavingChangesToPostgresSQL_ShouldHaveBoardWithGenerationPersisted()
    {
        // Arrange
        var initialState = new[,]
        {
            { true, true, false },
            { false, true, false },
            { false, false, false }
        };

        // Act
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<BoardDbContext>();

            var board = new Board(initialState);
            
            await context.Set<Board>().AddAsync(board);
            
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
            board!.Generations.Should().NotBeEmpty();
        }
    }
}