# Task Completion Checklist

## When a Development Task is Completed

### 1. Code Quality Checks
```bash
# Build the project to check for compilation errors
dotnet build

# Format code (if formatter is configured)
dotnet format

# Run static analysis (if configured)
dotnet build --verbosity normal
```

### 2. Testing Requirements
```bash
# Run existing tests (if any)
dotnet test

# Manual testing via Swagger UI
# Visit: https://localhost:7000/swagger
```

### 3. Database Verification
- Ensure database connections are working
- Verify data integrity across PostgreSQL and MongoDB
- Test with sample data if making data-related changes

### 4. Configuration Validation
- Check that appsettings.json is properly configured
- Verify connection strings are correct
- Ensure environment-specific settings work

### 5. API Documentation
- Update Swagger documentation if API contracts change
- Test all endpoints via Swagger UI
- Verify request/response models are correct

### 6. Error Handling Verification
- Test error scenarios and edge cases
- Verify proper HTTP status codes are returned
- Check that errors don't expose sensitive information

### 7. Performance Considerations
- Check for potential N+1 query issues
- Verify async/await usage where appropriate
- Monitor memory usage with large datasets

## Pre-Commit Checklist

### Code Review
- [ ] No hardcoded values added
- [ ] Proper error handling implemented
- [ ] Async/await used for I/O operations
- [ ] Interfaces used for dependencies
- [ ] Business logic not in controllers
- [ ] Proper logging added
- [ ] Input validation implemented

### Functionality
- [ ] All endpoints tested via Swagger
- [ ] Database operations working correctly
- [ ] Error scenarios handled gracefully
- [ ] No breaking changes to existing API

### Documentation
- [ ] Code comments added where needed
- [ ] API documentation updated
- [ ] README updated if needed

## Known Limitations (Demo Project)
Since this is a demo project with intentional tech debt:
- Some anti-patterns are expected to remain
- Not all best practices need to be implemented
- Focus on demonstrating specific improvements
- Document what improvements were made and why