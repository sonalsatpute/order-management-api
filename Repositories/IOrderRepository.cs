using OrderManagementAPI.Models;

namespace OrderManagementAPI.Repositories;

/// <summary>
/// Repository interface for Order entity operations
/// Provides abstraction over data storage implementation
/// </summary>
public interface IOrderRepository
{
    // Query operations
    Task<IEnumerable<Order>> GetAllAsync();
    Task<IEnumerable<Order>> GetByStatusAsync(string status);
    Task<IEnumerable<Order>> GetRecentOrdersAsync(int days = 30);
    Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Order>> GetArchivedOrdersAsync(int archiveDays = 90);
    Task<Order?> GetByIdAsync(string id);
    Task<IEnumerable<Order>> GetHighValueOrdersAsync(decimal threshold);
    
    // Command operations
    Task<Order> CreateAsync(Order order);
    Task<Order> UpdateAsync(Order order);
    Task<bool> UpdateStatusAsync(string id, string newStatus);
    Task<bool> DeleteAsync(string id);
    
    // Business query operations
    Task<decimal> GetMonthlyRevenueAsync(int month, int year);
    Task<int> GetOrderCountByStatusAsync(string status);
    Task<IEnumerable<Order>> GetOrdersNeedingProcessingAsync();
}