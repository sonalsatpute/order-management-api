# Suggested Commands for Order Management API

## Development Commands

### Basic Project Commands
```bash
# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the application (development)
dotnet run

# Run with specific environment
dotnet run --environment Development
```

### Database Commands
```bash
# Add Entity Framework migration (if needed)
dotnet ef migrations add <MigrationName>

# Update database with migrations
dotnet ef database update

# Drop database (for reset)
dotnet ef database drop
```

### Testing Commands
```bash
# Run all tests (if tests exist)
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Quality Commands
```bash
# Format code
dotnet format

# Static analysis (if configured)
dotnet build --verbosity normal
```

## Windows System Commands

### File Operations
```cmd
# List directory contents
dir
ls  # if Git Bash/WSL

# Find files
dir /s *.cs
findstr /s "pattern" *.cs

# Navigate directories
cd <directory>
cd ..
cd \
```

### Git Commands
```bash
git status
git add .
git commit -m "message"
git push
git pull
git branch
git checkout <branch>
```

## API Testing

### Local Development
- **Base URL**: https://localhost:7000
- **Swagger UI**: https://localhost:7000/swagger
- **API Endpoints**: https://localhost:7000/api/orders

### Database Setup Required
1. **PostgreSQL**: localhost:5432, database: OrderManagement
2. **MongoDB**: localhost:27017, database: OrderManagementDB

## Package Management
```bash
# Add package
dotnet add package <PackageName>

# Remove package
dotnet remove package <PackageName>

# List packages
dotnet list package

# Update packages
dotnet add package <PackageName> --version <Version>
```