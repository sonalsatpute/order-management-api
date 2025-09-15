using OrderManagementAPI.Models;

namespace OrderManagementAPI.Repositories;

/// <summary>
/// Composite repository that coordinates between MongoDB and PostgreSQL repositories
/// Business rule: High-value orders (>$500) go to PostgreSQL, low-value to MongoDB
/// </summary>
public class CompositeOrderRepository : IOrderRepository
{
    private readonly MongoOrderRepository _mongoRepository;
    private readonly PostgresOrderRepository _postgresRepository;
    private const decimal HIGH_VALUE_THRESHOLD = 500m;

    public CompositeOrderRepository(
        MongoOrderRepository mongoRepository,
        PostgresOrderRepository postgresRepository)
    {
        _mongoRepository = mongoRepository;
        _postgresRepository = postgresRepository;
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        var mongoTask = _mongoRepository.GetAllAsync();
        var postgresTask = _postgresRepository.GetAllAsync();
        
        await Task.WhenAll(mongoTask, postgresTask);
        
        var allOrders = new List<Order>();
        allOrders.AddRange(await mongoTask);
        allOrders.AddRange(await postgresTask);
        
        return allOrders.OrderByDescending(x => x.CreatedDate);
    }

    public async Task<IEnumerable<Order>> GetByStatusAsync(string status)
    {
        var mongoTask = _mongoRepository.GetByStatusAsync(status);
        var postgresTask = _postgresRepository.GetByStatusAsync(status);
        
        await Task.WhenAll(mongoTask, postgresTask);
        
        var allOrders = new List<Order>();
        allOrders.AddRange(await mongoTask);
        allOrders.AddRange(await postgresTask);
        
        return allOrders.OrderByDescending(x => x.CreatedDate);
    }

    public async Task<IEnumerable<Order>> GetRecentOrdersAsync(int days = 30)
    {
        var mongoTask = _mongoRepository.GetRecentOrdersAsync(days);
        var postgresTask = _postgresRepository.GetRecentOrdersAsync(days);
        
        await Task.WhenAll(mongoTask, postgresTask);
        
        var allOrders = new List<Order>();
        allOrders.AddRange(await mongoTask);
        allOrders.AddRange(await postgresTask);
        
        return allOrders.OrderByDescending(x => x.CreatedDate);
    }

    public async Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var mongoTask = _mongoRepository.GetOrdersByDateRangeAsync(startDate, endDate);
        var postgresTask = _postgresRepository.GetOrdersByDateRangeAsync(startDate, endDate);
        
        await Task.WhenAll(mongoTask, postgresTask);
        
        var allOrders = new List<Order>();
        allOrders.AddRange(await mongoTask);
        allOrders.AddRange(await postgresTask);
        
        return allOrders.OrderByDescending(x => x.CreatedDate);
    }

    public async Task<IEnumerable<Order>> GetArchivedOrdersAsync(int archiveDays = 90)
    {
        // Archived orders are typically in PostgreSQL
        return await _postgresRepository.GetArchivedOrdersAsync(archiveDays);
    }

    public async Task<Order?> GetByIdAsync(string id)
    {
        // Try MongoDB first (ObjectId format), then PostgreSQL
        var mongoOrder = await _mongoRepository.GetByIdAsync(id);
        if (mongoOrder != null)
        {
            return mongoOrder;
        }
        
        return await _postgresRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Order>> GetHighValueOrdersAsync(decimal threshold)
    {
        // High-value orders are primarily in PostgreSQL, but check both
        var mongoTask = _mongoRepository.GetHighValueOrdersAsync(threshold);
        var postgresTask = _postgresRepository.GetHighValueOrdersAsync(threshold);
        
        await Task.WhenAll(mongoTask, postgresTask);
        
        var allOrders = new List<Order>();
        allOrders.AddRange(await mongoTask);
        allOrders.AddRange(await postgresTask);
        
        return allOrders.OrderByDescending(x => x.Total);
    }

    public async Task<Order> CreateAsync(Order order)
    {
        // Business rule: Route based on order value
        if (order.Total > HIGH_VALUE_THRESHOLD)
        {
            return await _postgresRepository.CreateAsync(order);
        }
        else
        {
            return await _mongoRepository.CreateAsync(order);
        }
    }

    public async Task<Order> UpdateAsync(Order order)
    {
        // Try to update in the repository where it exists
        var existingOrder = await GetByIdAsync(order.Id ?? order.OrderId.ToString());
        if (existingOrder == null)
        {
            throw new InvalidOperationException($"Order not found: {order.Id ?? order.OrderId.ToString()}");
        }

        // Determine which repository based on existing order characteristics
        if (ShouldUsePostgres(existingOrder))
        {
            return await _postgresRepository.UpdateAsync(order);
        }
        else
        {
            return await _mongoRepository.UpdateAsync(order);
        }
    }

    public async Task<bool> UpdateStatusAsync(string id, string newStatus)
    {
        // Try both repositories, return true if either succeeds
        var mongoTask = _mongoRepository.UpdateStatusAsync(id, newStatus);
        var postgresTask = _postgresRepository.UpdateStatusAsync(id, newStatus);
        
        var results = await Task.WhenAll(mongoTask, postgresTask);
        return results.Any(result => result);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        // Try both repositories, return true if either succeeds
        var mongoTask = _mongoRepository.DeleteAsync(id);
        var postgresTask = _postgresRepository.DeleteAsync(id);
        
        var results = await Task.WhenAll(mongoTask, postgresTask);
        return results.Any(result => result);
    }

    public async Task<decimal> GetMonthlyRevenueAsync(int month, int year)
    {
        var mongoTask = _mongoRepository.GetMonthlyRevenueAsync(month, year);
        var postgresTask = _postgresRepository.GetMonthlyRevenueAsync(month, year);
        
        await Task.WhenAll(mongoTask, postgresTask);
        
        return await mongoTask + await postgresTask;
    }

    public async Task<int> GetOrderCountByStatusAsync(string status)
    {
        var mongoTask = _mongoRepository.GetOrderCountByStatusAsync(status);
        var postgresTask = _postgresRepository.GetOrderCountByStatusAsync(status);
        
        await Task.WhenAll(mongoTask, postgresTask);
        
        return await mongoTask + await postgresTask;
    }

    public async Task<IEnumerable<Order>> GetOrdersNeedingProcessingAsync()
    {
        var mongoTask = _mongoRepository.GetOrdersNeedingProcessingAsync();
        var postgresTask = _postgresRepository.GetOrdersNeedingProcessingAsync();
        
        await Task.WhenAll(mongoTask, postgresTask);
        
        var allOrders = new List<Order>();
        allOrders.AddRange(await mongoTask);
        allOrders.AddRange(await postgresTask);
        
        return allOrders.OrderBy(x => x.CreatedDate); // Oldest first for processing
    }

    private static bool ShouldUsePostgres(Order order)
    {
        // Business logic for determining storage location
        return order.Total > HIGH_VALUE_THRESHOLD || 
               !string.IsNullOrEmpty(order.Id) && !order.Id.Contains("ObjectId");
    }
}