using OrderManagementAPI.Models;

namespace OrderManagementAPI.Repositories
{
    /// <summary>
    /// Order-specific repository interface extending generic repository
    /// </summary>
    public interface IOrderRepository : IRepository<Order>
    {
        // Order-specific query methods
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(string status);
        Task<IEnumerable<Order>> GetOrdersByCustomerEmailAsync(string customerEmail);
        Task<IEnumerable<Order>> GetOrdersCreatedBetweenAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Order>> GetHighValueOrdersAsync(decimal minValue);
        Task<IEnumerable<Order>> GetOrdersByPriorityAsync(string priority);

        // Business-specific operations
        Task<decimal> GetTotalRevenueAsync();
        Task<decimal> GetRevenueByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<int> GetOrderCountByStatusAsync(string status);
        Task<IEnumerable<Order>> GetRecentOrdersAsync(int days = 30);
        Task<IEnumerable<Order>> GetPendingOrdersOlderThanAsync(int hours);
    }
}