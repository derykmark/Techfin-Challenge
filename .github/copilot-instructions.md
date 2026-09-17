# Copilot Instructions for TechfinChallenge

## Repository overview

This repository is a .NET 10 API challenge built around a Clean Architecture / DDD-inspired split:

- `src/TechfinChallenge.API`: ASP.NET Core API host, controllers, Swagger, middleware and startup configuration.
- `src/TechfinChallenge.Application`: use cases, DTOs and application contracts.
- `src/TechfinChallenge.Domain`: entities, value objects, domain exceptions, and business rules.
- `src/TechfinChallenge.Infrastructure`: Dapper repositories, JWT auth, BCrypt hashing, SQLite initialization, RabbitMQ/MassTransit messaging, and cache.
- `tests/TechfinChallenge.UnitTests` and `tests/TechfinChallenge.IntegrationTests`: automated coverage for the business logic and API endpoints.

The project is organized around feature boundaries (`Auth`, `Clientes`, `Transacoes`) rather than a single monolithic service. Keep new behavior aligned with that structure.

## Build, test, and validation commands

Use the repo root as the working directory.

```bash
# Restore dependencies
 dotnet restore

# Build the solution
 dotnet build TechfinChallenge.slnx --nologo

# Run the full suite
 dotnet test TechfinChallenge.slnx --nologo

# Run only unit tests
 dotnet test tests/TechfinChallenge.UnitTests/TechfinChallenge.UnitTests.csproj --nologo

# Run only integration tests
 dotnet test tests/TechfinChallenge.IntegrationTests/TechfinChallenge.IntegrationTests.csproj --nologo

# Run a single test by name (xUnit)
 dotnet test tests/TechfinChallenge.UnitTests/TechfinChallenge.UnitTests.csproj --nologo --filter "FullyQualifiedName~NomeDoTeste"

# Run a single test class or namespace subset
 dotnet test tests/TechfinChallenge.UnitTests/TechfinChallenge.UnitTests.csproj --nologo --filter "FullyQualifiedName~TechfinChallenge.Application.UseCases.Auth"
```

There is no dedicated lint script or formatter configuration in this repository. The normal validation flow is `dotnet build` + `dotnet test`.

To run the API locally:

```bash
 dotnet run --project src/TechfinChallenge.API/TechfinChallenge.API.csproj
```

The app is configured to expose Swagger in development and uses JWT auth for the `Clientes` and `Transacoes` endpoints.

## Big-picture architecture and runtime behavior

### 1. API -> Application -> Domain -> Infrastructure

The dependency direction is intentional:

- API depends on Application and Infrastructure.
- Application depends on Domain.
- Infrastructure depends on Application and Domain.
- Domain does not depend on the outer layers.

When changing behavior, prefer placing logic in the relevant use case under `Application/UseCases/...`, not in the controller itself.

### 2. Authentication and authorization

Authentication is handled by:

- `AuthController` for `registrar` and `login`
- `AutenticarUsuarioUseCase` for credentials verification
- `JwtTokenService` for JWT generation and expiration
- `DependencyInjection` for JWT bearer configuration

JWT tokens are required for the client and transaction resources. Secrets and issuer/audience values are configured in `appsettings.json` under `JwtSettings`.

### 3. Persistence and database setup

The project uses SQLite in-memory via `DbConnectionFactory` and Dapper queries from repository classes.

Important pieces:

- `DbConnectionFactory` opens a shared in-memory SQLite connection.
- `DatabaseInitializer` creates the `Usuarios`, `Clientes` and `Transacoes` tables at startup.
- Repositories under `src/TechfinChallenge.Infrastructure/Persistence/Repositories` perform the actual persistence logic.

When adding new persistence logic, follow the existing repository pattern and keep SQL queries close to the repository that owns the data.

### 4. Client and transaction workflows

The business rules are centered around `Cliente` and `Transacao` entities in the Domain layer:

- clients cannot be duplicated by CPF
- negative client limits are rejected
- transactions can only target existing clients
- approved transactions generate a GUID and reduce the client limit
- rejected transactions return a `NEGADO` response without persisting an approved transaction

The flow is implemented in the corresponding use cases under `Application/UseCases/Clientes` and `Application/UseCases/Transacoes`.

### 5. Messaging and async limit update

Approved transactions publish an event through an `IEventPublisher` implementation. The messaging setup is in `DependencyInjection` using MassTransit and RabbitMQ.

Relevant runtime pieces:

- `AutorizarTransacaoUseCase` publishes `TransacaoAutorizadaEvent`
- `TransacaoAutorizadaConsumer` consumes the event
- `AtualizarLimiteClienteUseCase` applies the client-limit deduction asynchronously

If a feature touches transaction approval, assume the message flow may be involved. Do not bypass the use case + event pattern unless the change is explicitly scoped to a direct fix.

### 6. Error handling and response contracts

The API uses a centralized exception middleware:

- `ExceptionHandlingMiddleware` catches domain exceptions and returns JSON `ErroResponse` payloads
- controllers are intentionally thin and return DTO responses from use cases
- non-domain failures bubble to a generic 500 response

Keep new validation failures aligned with `DomainException` and the existing `ErroResponse` contract instead of returning ad-hoc error payloads.

## Key conventions in this repository

- Keep controllers thin: they should delegate to use cases and return DTOs or typed response objects.
- Business rules belong in the Domain or Application layer; avoid putting validation logic directly in the API controller.
- Use the existing `*UseCase` naming pattern (`CadastrarClienteUseCase`, `ListarClientesUseCase`, `AutorizarTransacaoUseCase`).
- Prefer repository and Dapper patterns already used in Infrastructure; the database access layer is a strong convention here.
- `JsonNamingPolicy.CamelCase` is configured globally for API responses. Keep new DTO property names consistent with that convention.
- Auth endpoints are public, while `Clientes` and `Transacoes` endpoints are protected by `[Authorize]`.
- In-memory SQLite is used for local execution; test and runtime behavior rely on the same initialization flow via `DatabaseInitializer`.
- RabbitMQ must be running for the async approval flow to work in a live environment; do not assume the broker is optional when troubleshooting runtime issues.

## Relevant repo files to read first

When working in this codebase, these are the most useful starting points:

- `README.md` for the challenge requirements and expected behaviors
- `src/TechfinChallenge.API/Program.cs` for service registration and middleware setup
- `src/TechfinChallenge.Infrastructure/Configuration/DependencyInjection.cs` for DI and infrastructure wiring
- `src/TechfinChallenge.Application/UseCases/*` for the business logic entry points
- `src/TechfinChallenge.Infrastructure/Persistence/DatabaseInitializer.cs` for the SQLite schema
- `src/TechfinChallenge.API/Middlewares/ExceptionHandlingMiddleware.cs` for API-level error handling

## Constraints and expectations

- The project is a coding challenge, not a large enterprise app; keep changes focused and aligned with the described business flow.
- Prefer minimal, surgical changes that fit the existing layered design.
- Most feature work should be implemented in the Application layer and supported by existing repositories, not by custom ad hoc services in the API project.
