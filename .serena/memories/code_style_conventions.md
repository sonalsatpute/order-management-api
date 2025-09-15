# Code Style and Conventions

## Current Project Conventions (Anti-Patterns)
The project demonstrates **poor** conventions that should be avoided:

### Naming Conventions
- **Classes**: PascalCase (correct)
- **Methods**: PascalCase (correct)
- **Properties**: PascalCase (correct)
- **Fields**: camelCase with underscore prefix `_fieldName` (correct)
- **Variables**: camelCase (correct)

### Code Organization Issues
- **Fat Controllers**: Controllers contain business logic (❌ anti-pattern)
- **Mixed Concerns**: Data access, business logic, and HTTP concerns combined
- **No Interfaces**: Direct concrete class dependencies (❌ tight coupling)
- **Magic Numbers**: Hardcoded values throughout code (❌ anti-pattern)

### Data Access Anti-Patterns
- **Direct DbContext Usage**: Controllers directly use data contexts
- **Mixed Database Logic**: PostgreSQL and MongoDB access in same methods
- **No Repository Pattern**: No abstraction over data access
- **Synchronous Operations**: No async/await usage (❌ blocking operations)

### Error Handling Issues
- **Exception Swallowing**: Empty catch blocks with no logging
- **Generic Error Messages**: "Something went wrong" responses
- **No Structured Logging**: Console.WriteLine instead of proper logging

### Validation Problems
- **No Input Validation**: Missing data validation attributes
- **Business Rules in Controllers**: Validation logic scattered throughout
- **Primitive Obsession**: Strings instead of value objects or enums

## .NET Best Practices (What Should Be Done)

### Recommended Patterns
1. **Repository Pattern**: Abstract data access
2. **Dependency Injection**: Use interfaces for loose coupling
3. **Async/Await**: All I/O operations should be asynchronous
4. **Domain-Driven Design**: Rich domain models with behavior
5. **Clean Architecture**: Separate concerns into layers

### Proper Error Handling
```csharp
// Good: Proper error handling with logging
try
{
    await repository.SaveAsync(order);
    logger.LogInformation("Order {OrderId} created successfully", order.Id);
    return Ok(order);
}
catch (ValidationException ex)
{
    logger.LogWarning("Validation failed for order: {ValidationErrors}", ex.Errors);
    return BadRequest(ex.Errors);
}
catch (Exception ex)
{
    logger.LogError(ex, "Unexpected error creating order");
    return StatusCode(500, "An unexpected error occurred");
}
```

### Configuration Management
- Use `IConfiguration` for settings instead of hardcoded values
- Use `IOptions<T>` pattern for typed configuration
- Store sensitive data in user secrets or environment variables

### Testing Considerations
- Controllers should be thin and easily testable
- Business logic should be in services with interfaces
- Use dependency injection for better unit testing