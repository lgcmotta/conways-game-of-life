namespace Conways.GameOfLife.IntegrationTests.Features.FinalGeneration;

public class FinalGenerationTests : IClassFixture<ConwaysGameOfLifeWebApplicationFactory>
{
    private readonly ConwaysGameOfLifeWebApplicationFactory _factory;

    public FinalGenerationTests(ConwaysGameOfLifeWebApplicationFactory factory)
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

    public static TheoryData<string?, int> GetFinalGenerationQueryInputsForValidationFailedException()
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
    [MemberData(nameof(GetFinalGenerationQueryInputsForValidationFailedException))]
    public async Task FinalGeneration_WhenBoardIdIsNullOrEmptyOrMaxAttemptsIsNegativeOrZero_ShouldThrowValidationFailedException(string? boardId, int maxAttempts)
    {
        using var scope = _factory.Services.CreateScope();
        
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        
        // Act
        async Task RequestQuery() => await mediator.Send(new FinalGenerationQuery(boardId!, maxAttempts)); 

        // Assert
        await Assert.ThrowsAsync<ValidationFailedException>(RequestQuery);
    }

    [Fact]
    public async Task FinalGeneration_WhenBoardDoesNotExists_ShouldThrowBoardNotFoundException()
    {
        // Arrange
        const int maxAttempts = 3;
        using var scope = _factory.Services.CreateScope();
        
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var hashIds = scope.ServiceProvider.GetRequiredService<IHashids>();

        var boardId = hashIds.EncodeLong(1234);
        
        // Act
        async Task RequestQuery() => await mediator.Send(new FinalGenerationQuery(boardId, maxAttempts)); 
        
        // Assert
        await Assert.ThrowsAsync<BoardNotFoundException>(RequestQuery);
    }

    [Fact]
    public async Task FinalGeneration_WhenBoardExistsButCantReachStableGenerationAfterAttempts_ShouldThrowUnstableBoardException()
    {
        // Arrange
        const int maxAttempts = 2;
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
        async Task RequestQuery() => await mediator.Send(new FinalGenerationQuery(boardId, maxAttempts));
        
        // Assert
        await Assert.ThrowsAsync<UnstableBoardException>(RequestQuery);
    }

    [Fact]
    public async Task FinalGeneration_WhenBoardExistsAndStableStateIsWithinMaxAttempts_ShouldReturnFinalGenerationForBoard()
    {
        // Arrange
        const int maxAttempts = 5;
        var firstGeneration = new[,]
        {
            { false, false, false, false, false, false },
            { false, true, true, false, false, false },
            { false, true, false, false, false, false },
            { false, false, false, true, false, false },
            { false, false, false, true, true, false },
            { false, false, false, false, false, false }
        };

        var expectedFinalGeneration = new bool[][]
        {
            [false, false, false, false, false, false], 
            [false, true, true, false, false, false],
            [true, false, false, true, false, false], 
            [false, true, true, false, false, false],
            [false, false, false, false, false, false], 
            [false, false, false, false, false, false]
        };
        
        var boardId = await SeedBoard(firstGeneration);
        
        using var scope = _factory.Services.CreateScope();
        
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        
        // Act
        var response = await mediator.Send(new FinalGenerationQuery(boardId, maxAttempts));
        
        // Assert
        response.Should().NotBeNull();
        response.Stable.Should().BeTrue();
        response.FinalGeneration.Should().BeEquivalentTo(expectedFinalGeneration);
    }

    [Fact]
    public async Task FinalGeneration_WhenRequestingUsingAPIWithinReachableMaxAttempts_ShouldRespondWithFinalStableGeneration()
    {
        // Arrange
        const int maxAttempts = 5;
        
        var firstGeneration = new[,]
        {
            { false, false, false, false, false, false },
            { false, true, true, false, false, false },
            { false, true, false, false, false, false },
            { false, false, false, true, false, false },
            { false, false, false, true, true, false },
            { false, false, false, false, false, false }
        };
        
        var expectedFinalGeneration = new bool[][]
        {
            [false, false, false, false, false, false], 
            [false, true, true, false, false, false],
            [true, false, false, true, false, false], 
            [false, true, true, false, false, false],
            [false, false, false, false, false, false], 
            [false, false, false, false, false, false]
        };
        
        var boardId = await SeedBoard(firstGeneration);

        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = _factory.Server.BaseAddress
        });
        
        // Act
        var response = await client.GetAsync($"/api/v1/boards/{boardId}/generations/final?maxAttempts={maxAttempts}");

        var body = await response.Content.ReadFromJsonAsync<FinalGenerationResponse>();

        // Assert
        body.Should().NotBeNull();
        body!.Stable.Should().BeTrue();
        body.FinalGeneration.Should().BeEquivalentTo(expectedFinalGeneration);
    }

    [Fact]
    public async Task FinalGeneration_WhenMaxAttemptsIsNotProvidedWithQueryString_ShouldUseFallbackValueFromConfiguration()
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
        
        var expectedFinalGeneration = new bool[][]
        {
            [false, false, false, false, false, false], 
            [false, true, true, false, false, false],
            [true, false, false, true, false, false], 
            [false, true, true, false, false, false],
            [false, false, false, false, false, false], 
            [false, false, false, false, false, false]
        };
        
        var boardId = await SeedBoard(firstGeneration);

        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = _factory.Server.BaseAddress
        });
        
        // Act
        var response = await client.GetAsync($"/api/v1/boards/{boardId}/generations/final");

        var body = await response.Content.ReadFromJsonAsync<FinalGenerationResponse>();

        // Assert
        body.Should().NotBeNull();
        body!.Stable.Should().BeTrue();
        body.FinalGeneration.Should().BeEquivalentTo(expectedFinalGeneration);
    }

    [Fact]
    public async Task FinalGeneration_WhenRequestingUsingAPIWithStableStateOutOfMaxAttemptsReach_ShouldRespondWithUnprocessableEntity()
    {
        // Arrange
        const int maxAttempts = 2;
        
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
        var response = await client.GetAsync($"/api/v1/boards/{boardId}/generations/final?maxAttempts={maxAttempts}");

        var body = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        body.Should().NotBeNull();
        body!.Errors.Should().Contain($"Board with id '{boardId}' failed to reach stable state after {maxAttempts} attempts");
    }
}