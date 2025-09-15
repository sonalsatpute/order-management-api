# TypeScript Code Style Guide

## Naming Conventions

### Functions and Variables
- Use camelCase: `calculateTotal()`, `userName`

### Classes and Interfaces
- Use PascalCase: `UserService`, `PaymentProcessor`
- Prefix interfaces with 'I' (optional): `IUserRepository`

### Constants and Enums
- Use UPPER_SNAKE_CASE for constants: `MAX_RETRY_COUNT`
- Use PascalCase for enums: `UserStatus`, `PaymentType`

### Types and Generics
- Use PascalCase: `UserModel`, `ApiResponse<T>`

## Formatting

### Indentation
- Use 2 spaces for indentation
- Align nested structures

### Strings
- Use single quotes: `'Hello World'`
- Use template literals for interpolation: `` `Hello ${name}` ``

### Semicolons
- Always use semicolons at end of statements

## Angular Specific

### Components
- Use PascalCase with Component suffix: `UserProfileComponent`
- File names use kebab-case: `user-profile.component.ts`

### Services
- Use PascalCase with Service suffix: `UserService`
- File names use kebab-case: `user.service.ts`

### Modules
- Use PascalCase with Module suffix: `SharedModule`
- File names use kebab-case: `shared.module.ts`

### Directives and Pipes
- Use camelCase with descriptive names
- File names use kebab-case

## Type Definitions

### Strict Type Checking
- Always define return types for functions
- Use explicit types for complex objects
- Prefer interfaces over types for object shapes

### Generic Types
- Use descriptive generic parameter names: `<TResponse>` instead of `<T>`

### Null Safety
- Use strict null checks
- Use optional chaining: `user?.profile?.name`
- Use nullish coalescing: `value ?? defaultValue`