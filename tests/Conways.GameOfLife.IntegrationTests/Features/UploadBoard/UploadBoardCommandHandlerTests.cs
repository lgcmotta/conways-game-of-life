namespace Conways.GameOfLife.IntegrationTests.Features.UploadBoard;

public class UploadBoardCommandHandlerTests : IClassFixture<ConwaysGameOfLifeWebApplicationFactory>
{
    private readonly ConwaysGameOfLifeWebApplicationFactory _factory;

    public UploadBoardCommandHandlerTests(ConwaysGameOfLifeWebApplicationFactory factory)
    {
        _factory = factory;
    }
    
    [Fact]
    public async Task UploadBoard_WhenBoardIsValidSize_ShouldSaveAndEncodeBoardId()
    {
        // Arrange
        var firstGeneration = new bool[][]
        {
            [false, true, false],
            [true, true, false],
            [false, false, false]
        };

        using var scope = _factory.Services.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        
        // Act
        var response = await mediator.Send(new UploadBoardCommand(firstGeneration));

        // Assert
        response.BoardId.Should().NotBeEmpty();
        response.BoardId.Should().HaveLength(11);
    }

    [Fact]
    public async Task MethodName_WhenSomethingHappens_ShouldReturnSomething()
    {
        // Arrange
        var firstGeneration = new bool[][]
        {
            [false, true],
            [true, true, false],
            [false, false]
        };
        
        using var scope = _factory.Services.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        
        // Act
        async Task SendCommand() => await mediator.Send(new UploadBoardCommand(firstGeneration));

        // Assert
        await Assert.ThrowsAsync<ValidationFailedException>(SendCommand);
    }

    [Fact]
    public async Task MethodName_WhenSomethingHappens_ShouldReturnSomething2()
    {
        // Arrange
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = _factory.Server.BaseAddress
        });
        
        var firstGeneration = new bool[][]
        {
            [false, true, false],
            [true, true, false],
            [false, false, false]
        };
        
        var jsonString = JsonSerializer.Serialize(new UploadBoardCommand(firstGeneration));
        
        // Act
        var response = await client.PostAsync("/api/boards", new StringContent(jsonString, Encoding.UTF8, "application/json"));
        
        var boardId = await response.Content.ReadFromJsonAsync<UploadBoardResponse>();

        // Assert
        boardId.Should().NotBeNull();
        boardId!.BoardId.Should().NotBeEmpty();
        boardId.BoardId.Should().HaveLength(11);
    }
}