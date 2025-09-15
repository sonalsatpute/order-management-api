# .NET Tech Stack

## Context

.NET tech stack defaults for Agent OS projects, overridable in project-specific `.agent-os/product/tech-stack.md`.

## Backend

- Framework: .NET 8.0+
- Language: C# 12+
- Architecture: Domain Driven Design (DDD)
- API Pattern: RESTful Web API
- Authentication: JWT with ASP.NET Core Identity
- Dependency Injection: Built-in DI Container
- Testing Framework: xUnit
- Mocking: NSubstitute
- Assertions: FluentAssertions
- Test Data: Bogus (Faker)
- Package Manager: NuGet

## Database

- Primary Database: PostgreSQL 17+
- ORM: Entity Framework Core
- Database Migrations: Flyway

## Frontend

- Framework: Angular 18+
- Language: TypeScript 5.0+
- Build Tool: Angular CLI
- Package Manager: npm
- Node Version: 22 LTS
- CSS Framework: TailwindCSS 4.0+
- UI Components: Angular Material
- Icons: Angular Material Icons
- State Management: NgRx (for complex apps)
- Testing Framework: Jasmine + Karma

## Infrastructure

- Application Hosting: Digital Ocean App Platform/Droplets
- Hosting Region: Primary region based on user base
- Database Hosting: Digital Ocean Managed PostgreSQL
- Database Backups: Daily automated
- Asset Storage: Amazon S3
- CDN: CloudFront
- Asset Access: Private with signed URLs

## DevOps

- CI/CD Platform: GitHub Actions
- CI/CD Trigger: Push to main/staging branches
- Tests: Run before deployment
- Production Environment: main branch
- Staging Environment: staging branch
- Containerization: Docker (optional)