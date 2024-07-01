# Conway's Game of Life - Example Code

[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=lgcmotta_conways-game-of-life&metric=coverage)](https://sonarcloud.io/summary/new_code?id=lgcmotta_conways-game-of-life)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=lgcmotta_conways-game-of-life&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=lgcmotta_conways-game-of-life)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=lgcmotta_conways-game-of-life&metric=bugs)](https://sonarcloud.io/summary/new_code?id=lgcmotta_conways-game-of-life)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=lgcmotta_conways-game-of-life&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=lgcmotta_conways-game-of-life)

This repository holds a test propose for a job position. The challenge was to implement an API for [Conway's Game of Life](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life).

The requirements are:

1. Allows uploading a new board state, returns id of board. 
2. Get next state for board, returns next state
3. Gets x number of states away for board
4. Gets final state for board. If board doesn't go to conclusion after x number of attempts, returns
   error

> The service you write should be able to restart/crash/etc... but retain the state of the boards.

> The code you write should be production ready. You don’t need to implement any
authentication/authorization. Be prepared to show how your code meets production ready
requirements.

I have chosen a domain-driven design approach to model the Board's domain using the terminology of `Generation` for the "board's state", since it's mostly seen when reading about the Game of Life.
Another decision was to work with Vertical Slice Architecture, since it's a small project and it quickly reveals the application intent when looking at the code.

## Environment :whale:

To work with this project you will need Docker (with Docker Compose), .NET 8 SDK and the IDE of your choice. I'm going with JetBrains Rider.

The `docker-compose.yml` contains the following containers: PostgreSQL, Jaeger, Loki, Grafana, Prometheus and OTEL Collector (contrib).

This application produces logs using the `Serilog.Sinks.OpenTelemetry` package to the OTEL Collector (contrib), which then are exported to Loki.
Traces and Metrics are collected and exported using `OpenTelemetry` packages to the OTEL Collector (contrib), which then are exported to Jaeger and Prometheus respectively.

To run the project you must start the containers using:

```shell
docker compose up -d
```

After the containers are up and running, you can start the project with your IDE's debugger or running the following command:

```shell
dotnet run --project ./src/Conways.GameOfLife.API/ --configuration Release
```

This projects has OpenAPI support, and it uses Swagger UI. The Swagger UI should be available at http://localhost:5224/swagger/index.html

If desired, you can also run this application using https with:

```shell
dotnet run --project ./src/Conways.GameOfLife.API/ --configuration Release --launch-profile https
```

The Swagger UI with https support should be available at https://localhost:7203/swagger/index.html

**Note**

> A developer's certificate may be required to run using https, try run this command `dotnet dev-certs https --trust` or 
> check the Microsoft's documentation about [dotnet dev-certs](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-dev-certs). 

## Architectural and Design Decisions :building_construction:

### Rich Domain Modeling
The application domain was designed using a rich domain model approach, which employs ubiquitous language to ensure clarity and shared understanding. 
This often involves wrapping native data types within classes or records to represent meaningful concepts in the application. 
For instance, the board's generation is encapsulated within a class that internally handles its value as a jagged array of booleans. 
To facilitate this approach, I utilized [implicit conversion operators](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/user-defined-conversion-operators) 
and [overloaded operators](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/operator-overloading) to seamlessly convert the generation to and from a jagged array of booleans.

### Database Choice and ORM
Given the need to handle multidimensional arrays efficiently, PostgreSQL was selected for its native support for storing columns as multidimensional arrays. 
Configuring **EF Core** to support this was straight forward, leveraging my experience from previous projects involving **EF Core** and rich domain models. 
**EF Core** excels as an ORM by mapping objects, including complex and well-encapsulated ones, to database structures. 
This ensures that the application domain remains decoupled from the database schema, maintaining a clean separation of concerns.

### Vertical Slices Architecture
To maintain an organized codebase, the project follows the Vertical Slices Architecture pattern. 
This approach ensures that all components related to a specific feature are contained within a single folder. 
This structure aligns well with domain-driven design principles, where the domain's aggregate is responsible for managing its internal state. 
Features interact with the aggregate's exposed API to fulfill business requirements, providing a clear and maintainable structure for development and feature modification.

### Testing Strategy
The project incorporates both unit tests and integration tests to ensure robust functionality:

#### Unit Tests 

These tests verify the correctness of the domain logic, ensuring that individual components behave as expected.

#### Integration Tests 

These tests validate the end-to-end behavior of application features, covering the entire workflow from receiving HTTP requests to interacting with the database. 
Integration tests help achieve high code coverage by testing workflows holistically rather than testing each small piece of code in isolation.
The use of the `xUnit` package, combined with `IClassFixture<T>` and `WebApplicationFactory<T>`, along with the `Testcontainers` package, 
has streamlined the testing process, making it easier to set up and execute tests.

## Contribute :wave:

I welcome contributions to this project. Although there is no formal contribution guide, 
feel free to [open an issue](https://github.com/lgcmotta/conways-game-of-life/issues/new) to start a discussion. 
I’m more than happy to collaborate and discuss code design, potential enhancements, and improvements
Please ensure your pull request uses the template and have a clear description of the issue and how your changes resolve it.