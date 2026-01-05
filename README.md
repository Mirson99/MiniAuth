# MiniAuth

A production-ready authentication API built with .NET 10, featuring JWT tokens with refresh token rotation, permission-based authorization, and Clean Architecture.

## Features

- User registration with strong password validation
- JWT authentication (15-minute access tokens)
- Refresh token rotation (7-day refresh tokens)
- Permission-based authorization (RBAC)
- Argon2id password hashing (OWASP recommended)
- CQRS pattern with MediatR
- FluentValidation with pipeline behavior
- Global exception handling
- Clean Architecture (4 layers)
- Integration tests with SQLite

## Tech Stack

- .NET 9
- PostgreSQL (Docker)
- MediatR
- FluentValidation
- Entity Framework Core
- JWT Bearer Authentication
- Argon2id
- xUnit + FluentAssertions

## Getting Started

### Prerequisites

- .NET 10 SDK
- Docker Desktop

### Installation

1. Clone the repository:
```bash
git clone https://github.com/Mirson99/MiniAuth.git
cd MiniAuth
```

2. Start PostgreSQL:
```bash
docker-compose up -d
```

3. Run migrations:
```bash
dotnet ef database update --project src/MiniAuth.Infrastructure --startup-project src/MiniAuth.API
```

4. Run the application:
```bash
dotnet run --project src/MiniAuth.API
```

5. Open Swagger:
```
http://localhost:5012/swagger
```

## API Endpoints

### Authentication
- `POST /auth/register` - Register new user
- `POST /auth/login` - Login with email/password
- `POST /auth/refresh-token` - Refresh access token

## Testing

Run integration tests:
```bash
dotnet test
```

Tests use SQLite in-memory database for fast, isolated testing.

## Project Structure
```
MiniAuth/
├── src/
│   ├── MiniAuth.API/              # Presentation layer
│   ├── MiniAuth.Application/       # Business logic (CQRS)
│   ├── MiniAuth.Domain/            # Entities & Enums
│   └── MiniAuth.Infrastructure/    # Data access & Services
├── tests/
│   └── MiniAuth.IntegrationTests/  # Integration tests
├── docker-compose.yml              # PostgreSQL setup
└── MiniAuth.sln
```

## Key Design Patterns

- **Clean Architecture** - Dependency inversion, separation of concerns
- **CQRS** - Command Query Responsibility Segregation with MediatR
- **Repository Pattern** - Data access abstraction
- **Pipeline Behavior** - Automatic validation with FluentValidation
