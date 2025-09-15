# C# Code Style Guide

## Naming Conventions

### Methods and Properties
- Use PascalCase: `CalculateTotal()`, `UserName`

### Variables and Parameters
- Use camelCase: `userName`, `totalAmount`

### Classes and Interfaces
- Use PascalCase: `UserService`, `IPaymentProcessor`
- Prefix interfaces with 'I': `IRepository`, `IValidator`

### Constants and Fields
- Use PascalCase: `MaxRetryCount`, `DefaultTimeout`
- Private fields prefix with underscore: `_repository`, `_logger`

## Formatting

### Indentation
- Use 4 spaces for indentation
- Align opening and closing braces

### Braces
- Opening brace on new line (Allman style)
```csharp
if (condition)
{
    // code here
}
```

### String Formatting
- Use string interpolation: `$"Hello {name}"`
- Use verbatim strings for paths: `@"C:\Folder\File.txt"`

## Domain Driven Design

### Entity Structure
- Keep entities focused on domain logic
- Use value objects for simple types
- Implement domain events for cross-aggregate communication

### Repository Pattern
- Define interfaces in domain layer
- Implement repositories in infrastructure layer
- Use async/await for database operations

### Service Layer
- Keep application services thin
- Domain services contain business logic
- Use dependency injection for cross-cutting concerns