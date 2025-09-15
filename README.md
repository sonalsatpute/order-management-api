# Order Management API - Legacy Demo Project

## Purpose
This is a **deliberately flawed** .NET 6 Web API created for Claude Code demonstration purposes. It contains multiple tech debt issues and anti-patterns commonly found in legacy codebases.

## 🚨 Known Tech Debt Issues

### Architecture Problems
- ❌ Fat controllers with business logic
- ❌ No repository pattern
- ❌ Mixed data access patterns (MongoDB + Postgres)
- ❌ Anemic domain models
- ❌ God classes and services

### Code Quality Issues
- ❌ Hardcoded connection strings and magic numbers
- ❌ No async/await patterns
- ❌ Poor error handling (swallowing exceptions)
- ❌ Missing validation
- ❌ No interfaces (tight coupling)

### Data Access Problems
- ❌ Direct DbContext usage in controllers
- ❌ N+1 query problems
- ❌ Inefficient data loading
- ❌ Mixed storage decisions in business layer

## Setup Instructions

### Prerequisites
- .NET 6 SDK
- PostgreSQL (localhost:5432)
- MongoDB (localhost:27017)

### Database Setup
```sql
-- PostgreSQL
CREATE DATABASE OrderManagement;
CREATE DATABASE OrderManagement_Dev;
```

### Running the Application
```bash
dotnet restore
dotnet run
```

Visit: https://localhost:7000/swagger

## Demo Scenarios

### Scenario 1: "Analyze this legacy codebase"
Start with exploring the project structure and identifying anti-patterns.

### Scenario 2: "Implement repository pattern"
Refactor data access to use proper repository pattern.

### Scenario 3: "Extract business logic from controller"
Move business rules to proper domain services.

### Scenario 4: "Add proper validation"
Implement FluentValidation for request validation.

## What NOT to Do
This project demonstrates what **not** to do in production code. Use it as a learning tool for refactoring exercises.