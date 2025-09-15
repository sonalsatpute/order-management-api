# Order Management API - Project Overview

## Purpose
This is a **deliberately flawed** .NET 6 Web API created for Claude Code demonstration purposes. It contains multiple tech debt issues and anti-patterns commonly found in legacy codebases. The project serves as a learning tool for refactoring exercises and demonstrates what **not** to do in production code.

## Tech Stack
- **Framework**: .NET 6 Web API
- **Primary Database**: PostgreSQL (for high-value orders >$500)
- **Secondary Database**: MongoDB (for low-value orders ≤$500)
- **ORM**: Entity Framework Core (for PostgreSQL)
- **MongoDB Driver**: MongoDB.Driver 2.17.1
- **API Documentation**: Swagger/OpenAPI
- **Validation**: FluentValidation.AspNetCore (available but not implemented)
- **Logging**: Serilog.AspNetCore (available but minimal usage)

## Project Structure
```
order-management-api/
├── Controllers/
│   └── OrdersController.cs       # Fat controller with mixed concerns
├── Data/
│   ├── MongoContext.cs          # MongoDB context
│   └── PostgresContext.cs       # EF Core PostgreSQL context
├── Models/
│   └── Order.cs                 # Anemic domain model with OrderItem
├── Services/
│   └── OrderService.cs          # God service with mixed responsibilities
├── Program.cs                   # Application startup with hardcoded configs
├── appsettings.json            # Configuration (has unused settings)
└── OrderManagementAPI.csproj   # Project dependencies
```

## Key Anti-Patterns Demonstrated
1. **Fat Controllers**: Business logic mixed with HTTP concerns
2. **No Repository Pattern**: Direct DbContext usage in controllers
3. **Mixed Data Access**: Dual database storage based on order value
4. **Anemic Domain Models**: Models with no behavior, just data
5. **God Classes**: Services doing too many things
6. **Hardcoded Values**: Magic numbers and connection strings
7. **Poor Error Handling**: Exception swallowing and generic errors
8. **No Async/Await**: Synchronous database operations
9. **Primitive Obsession**: Strings instead of enums for status/priority
10. **No Validation**: Missing input validation and business rules

## Intended Learning Scenarios
- Analyze legacy codebase for anti-patterns
- Implement repository pattern refactoring
- Extract business logic from controllers
- Add proper validation and error handling
- Convert to async/await patterns
- Implement proper domain models with behavior