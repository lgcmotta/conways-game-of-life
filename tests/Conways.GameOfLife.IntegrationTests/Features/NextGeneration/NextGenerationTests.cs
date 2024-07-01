namespace Conways.GameOfLife.IntegrationTests.Features.NextGeneration;

public class NextGenerationTests : IClassFixture<ConwaysGameOfLifeWebApplicationFactory>
{
    private readonly ConwaysGameOfLifeWebApplicationFactory _factory;

    public NextGenerationTests(ConwaysGameOfLifeWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private async Task<string> SeedBord(bool[,] firstGeneration)
    {
        using var scope = _factory.Services.CreateScope();
        
        var context = scope.ServiceProvider.GetRequiredService<BoardDbContext>();
        var hashIds = scope.ServiceProvider.GetRequiredService<IHashids>();
            
        var board = new Board(firstGeneration);

        await context.AddAsync(board);

        await context.SaveChangesAsync();

        return  hashIds.EncodeLong(board.Id);
    }
    
    public static TheoryData<string?> GetBoardIdsForValidationFailedException()
    {
        // ReSharper disable once UseCollectionExpression
        return new TheoryData<string?>
        {
            null,
            string.Empty
        };
    }

    [Fact]
    public async Task NextGeneration_WhenBoardExists_ShouldReturnNextGenerationResponse()
    {
        // Arrange
        var firstGeneration = new[,]
        {
            { true, true, false },
            { false, true, false },
            { false, false, false }
        };
        
        var expectedNextState = new bool[][]
        {
            [true, true, false],
            [true, true, false],
            [false, false, false]
        };

        var boardId = await SeedBord(firstGeneration);
        
        using var scope = _factory.Services.CreateScope();
        
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        
        // Act
        var response = await mediator.Send(new NextGenerationQuery(boardId));

        // Assert
        response.Generation.Should().BeEquivalentTo(expectedNextState);
    }

    [Fact]
    public async Task NextGeneration_WhenBoardDoesNotExists_ShouldThrowBoardNotFoundException()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var hashIds = scope.ServiceProvider.GetRequiredService<IHashids>();

        var boardId = hashIds.EncodeLong(1234);
        
        // Act
        async Task RequestQuery() => await mediator.Send(new NextGenerationQuery(boardId)); 
        
        // Assert
        await Assert.ThrowsAsync<BoardNotFoundException>(RequestQuery);
    }
    
    [Theory]
    [MemberData(nameof(GetBoardIdsForValidationFailedException))]
    public async Task NextGeneration_WhenBoardIdIsNullOrEmpty_ShouldThrowValidationFailedException(string? boardId)
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        
        // Act
        async Task RequestQuery() => await mediator.Send(new NextGenerationQuery(boardId!)); 

        // Assert
        await Assert.ThrowsAsync<ValidationFailedException>(RequestQuery);
    }

    [Fact]
    public async Task NextGeneration_WhenRequestingUsingAPI_ShouldRespondWithExpectedNextGeneration()
    {
        // Arrange
        var firstGeneration = new[,]
        {
            { true, true, false },
            { false, true, false },
            { false, false, false }
        };
        
        var expectedNextState = new bool[][]
        {
            [true, true, false],
            [true, true, false],
            [false, false, false]
        };

        var boardId = await SeedBord(firstGeneration);

        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = _factory.Server.BaseAddress
        });
        
        // Act
        var response = await client.GetAsync($"/api/boards/{boardId}/generations/next");

        var body = await response.Content.ReadFromJsonAsync<NextGenerationResponse>();

        // Assert
        body.Should().NotBeNull();
        body!.Generation.Should().BeEquivalentTo(expectedNextState);
    }
}   