# API Specification

This is the API specification for the spec detailed in @.agent-os/specs/2025-01-15-repository-pattern/spec.md

## Repository Interface APIs

### IOrderRepository Interface

**Purpose:** Unified interface for order data operations across both MongoDB and PostgreSQL storage systems
**Integration:** Replaces direct database context usage in OrdersController and OrderService

```csharp
public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(string id);
    Task<IEnumerable<Order>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Order>> GetByStatusAsync(string status);
    Task<IEnumerable<Order>> GetRecentOrdersAsync(int days = 30);
    Task<Order> CreateAsync(Order order);
    Task<Order> UpdateAsync(Order order);
    Task<bool> DeleteAsync(string id);
    Task<decimal> CalculateMonthlyRevenueAsync(int month, int year);
}
```

### IStorageStrategy Interface

**Purpose:** Configurable strategy for determining storage location based on business rules
**Integration:** Injected into CompositeOrderRepository for storage routing decisions

```csharp
public interface IStorageStrategy<T>
{
    StorageType DetermineStorage(T entity);
    StorageType DetermineStorageForRetrieval(string id);
}

public enum StorageType
{
    MongoDB,
    PostgreSQL,
    Both // For read operations that need to check both stores
}
```

## Controller Integration

### OrdersController Refactoring

**Purpose:** Replace direct database context dependencies with repository pattern
**Parameters:** Remove MongoContext and PostgresContext constructor parameters
**Response:** Maintain existing HTTP response formats and status codes
**Errors:** Enhanced error handling through repository exceptions

**Before (Current):**
```csharp
public OrdersController(MongoContext mongoContext, PostgresContext postgresContext, OrderService orderService)
```

**After (Repository Pattern):**
```csharp
public OrdersController(IOrderRepository orderRepository, ILogger<OrdersController> logger)
```

### Service Integration

**Purpose:** Refactor OrderService to use repository abstraction instead of direct context access
**Business Logic:** Maintain existing business logic while delegating data access to repositories
**Error Handling:** Implement proper exception handling for repository operations

**Method Signatures:**
- `GetRecentOrders(int days)` → Repository delegation with async pattern
- `CalculateMonthlyRevenue(int month, int year)` → Repository method call
- `ProcessHighValueOrders()` → Repository-based order retrieval and updates
- `ValidateAndCreateOrder(Order order)` → Repository CreateAsync usage

## Dependency Injection Configuration

**Purpose:** Configure repository dependencies in Program.cs
**Registration:** Register repository implementations with proper lifetimes
**Configuration:** Bind storage strategy settings from appsettings.json

```csharp
// Repository registrations
builder.Services.AddScoped<IMongoOrderRepository, MongoOrderRepository>();
builder.Services.AddScoped<IPostgresOrderRepository, PostgresOrderRepository>();
builder.Services.AddScoped<IOrderRepository, CompositeOrderRepository>();
builder.Services.AddSingleton<IStorageStrategy<Order>, OrderStorageStrategy>();

// Configuration binding
builder.Services.Configure<StorageSettings>(builder.Configuration.GetSection("StorageSettings"));
```