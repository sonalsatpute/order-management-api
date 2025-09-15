# Technical Stack

## Application Framework
- **Framework:** ASP.NET Core Web API 6.0
- **Language:** C# 10.0
- **Runtime:** .NET 6.0

## Database Systems
- **Primary Database:** PostgreSQL (Npgsql.EntityFrameworkCore.PostgreSQL 6.0.7)
- **Secondary Database:** MongoDB (MongoDB.Driver 2.17.1)
- **ORM:** Entity Framework Core 6.0.10

## Development Tools
- **API Documentation:** Swagger/OpenAPI (Swashbuckle.AspNetCore 6.2.3)
- **Validation:** FluentValidation.AspNetCore 11.2.2
- **Logging:** Serilog.AspNetCore 6.0.1

## Architecture Patterns
- **Repository Pattern:** For data access abstraction
- **CQRS:** Command Query Responsibility Segregation
- **Domain-Driven Design:** Rich domain models with proper encapsulation
- **Dependency Injection:** Built-in .NET DI container

## Code Quality
- **Principles:** SRP (Single Responsibility), KISS (Keep It Simple), DRY (Don't Repeat Yourself)
- **Testing:** nUnit with NSubstitute for mocking
- **Code Analysis:** Built-in .NET analyzers

## Development Environment
- **IDE:** Visual Studio 2022 / VS Code
- **Version Control:** Git
- **Package Manager:** NuGet

## Deployment
- **Containerization:** Docker support
- **Configuration:** appsettings.json with environment overrides
- **Health Checks:** Built-in ASP.NET Core health check middleware

## Legacy Transformation Stack
- **Current State:** Technical debt with anti-patterns
- **Target State:** Clean architecture with SOLID principles
- **Migration Strategy:** Incremental refactoring with continuous validation