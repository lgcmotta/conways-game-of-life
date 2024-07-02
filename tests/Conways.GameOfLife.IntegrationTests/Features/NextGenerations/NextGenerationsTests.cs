namespace Conways.GameOfLife.IntegrationTests.Features.NextGenerations;

public class NextGenerationsTests : IClassFixture<ConwaysGameOfLifeWebApplicationFactory>
{
    private readonly ConwaysGameOfLifeWebApplicationFactory _factory;

    public NextGenerationsTests(ConwaysGameOfLifeWebApplicationFactory factory)
    {
        _factory = factory;
    }
    
    private async Task<string> SeedBoard(bool[,] firstGeneration)
    {
        using var scope = _factory.Services.CreateScope();
        
        var context = scope.ServiceProvider.GetRequiredService<BoardDbContext>();
        var hashIds = scope.ServiceProvider.GetRequiredService<IHashids>();
            
        var board = new Board(firstGeneration);

        await context.AddAsync(board);

        await context.SaveChangesAsync();

        return  hashIds.EncodeLong(board.Id);
    }

    public static TheoryData<string?, int> GetNextGenerationsQueryInputsForValidationFailedException()
    {
        return new TheoryData<string?, int>
        {
            { null, 1 },
            { string.Empty, 1 },
            { "JzjO0ZW6D83", 0 },
            { "JzjO0ZW6D83", -1 }
        };
    }

    [Theory]
    [MemberData(nameof(GetNextGenerationsQueryInputsForValidationFailedException))]
    public async Task NextGenerations_WhenBoardIdIsNullOrEmptyOrGenerationsIsNegativeOrZero_ShouldThrowValidationFailedException(string? boardId, int generations)
    {
        using var scope = _factory.Services.CreateScope();
        
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        
        // Act
        async Task RequestQuery() => await mediator.Send(new NextGenerationsQuery(boardId!, generations)); 

        // Assert
        await Assert.ThrowsAsync<ValidationFailedException>(RequestQuery);
    }

    [Fact]
    public async Task NextGenerations_WhenBoardDoesNotExists_ShouldThrowBoardNotFoundException()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var hashIds = scope.ServiceProvider.GetRequiredService<IHashids>();

        var boardId = hashIds.EncodeLong(1234);
        
        // Act
        async Task RequestQuery() => await mediator.Send(new NextGenerationsQuery(boardId, 3)); 
        
        // Assert
        await Assert.ThrowsAsync<BoardNotFoundException>(RequestQuery);
    }

    [Fact]
    public async Task NextGenerations_WhenBoardExistsAndGenerationsAreValid_ShouldReturnNextGenerationsResponse()
    {
        // Arrange
        var firstGeneration = new[,]
        {
            { false, false, false, false, false, false },
            { false, true, true, false, false, false },
            { false, true, false, false, false, false },
            { false, false, false, true, false, false },
            { false, false, false, true, true, false },
            { false, false, false, false, false, false }
        };
        
        var boardId = await SeedBoard(firstGeneration);
        
        using var scope = _factory.Services.CreateScope();
        
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        
        // Act
        var response = await mediator.Send(new NextGenerationsQuery(boardId, 2));
        
        // Assert
        response.Should().NotBeNull();
        response.Generations.Should().HaveCount(3);
        response.Stable.Should().BeFalse();
    }

    [Fact]
    public async Task NextGenerations_WhenRequestingUsingAPI_ShouldRespondWithExpectedNextGenerationsCountIncludingTheFirst()
    {
        // Arrange
        var firstGeneration = new[,]
        {
            { false, false, false, false, false, false },
            { false, true, true, false, false, false },
            { false, true, false, false, false, false },
            { false, false, false, true, false, false },
            { false, false, false, true, true, false },
            { false, false, false, false, false, false }
        };
        var boardId = await SeedBoard(firstGeneration);

        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = _factory.Server.BaseAddress
        });
        
        // Act
        var response = await client.GetAsync($"/api/v1/boards/{boardId}/generations/{5}");

        var body = await response.Content.ReadFromJsonAsync<NextGenerationsResponse>();

        // Assert
        body.Should().NotBeNull();
        body!.Generations.Should().HaveCount(6);
        body!.Stable.Should().BeTrue();
    }
    
    [Fact]
    public async Task NextGenerations_WhenBoardDoesNotExistsRequestingUsingAPI_ShouldRespondNotFound()
    {
        // Arrange
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = _factory.Server.BaseAddress
        });

        using var scope = _factory.Services.CreateScope();

        var hashIds = scope.ServiceProvider.GetRequiredService<IHashids>();

        var boardId = hashIds.EncodeLong(1234);
        
        // Act
        var response = await client.GetAsync($"/api/v1/boards/{boardId}/generations/{5}");

        var body = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        body.Should().NotBeNull();
        body!.Errors.Should().Contain($"Board with Id '{boardId}' was not found");
    }
}