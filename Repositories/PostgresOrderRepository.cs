using Microsoft.EntityFrameworkCore;
using OrderManagementAPI.Data;
using OrderManagementAPI.Models;

namespace OrderManagementAPI.Repositories;

/// <summary>
/// PostgreSQL implementation of the Order repository
/// Handles orders stored in PostgreSQL (typically high-value orders)
/// </summary>
public class PostgresOrderRepository : IOrderRepository
{
    private readonly PostgresContext _context;

    public PostgresOrderRepository(PostgresContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await _context.Orders.ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetByStatusAsync(string status)
    {
        return await _context.Orders
            .Where(x => x.Status == status)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetRecentOrdersAsync(int days = 30)
    {
        var cutoffDate = DateTime.Now.AddDays(-days);
        return await _context.Orders
            .Where(x => x.CreatedDate >= cutoffDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Orders
            .Where(x => x.CreatedDate >= startDate && x.CreatedDate < endDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetArchivedOrdersAsync(int archiveDays = 90)
    {
        var archiveDate = DateTime.Now.AddDays(-archiveDays);
        return await _context.Orders
            .Where(x => x.CreatedDate < archiveDate)
            .ToListAsync();
    }

    public async Task<Order?> GetByIdAsync(string id)
    {
        // Handle both string and integer IDs for PostgreSQL
        if (int.TryParse(id, out int orderId))
        {
            return await _context.Orders
                .FirstOrDefaultAsync(x => x.OrderId == orderId);
        }
        
        // If string ID doesn't parse to int, try direct string comparison
        return await _context.Orders
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<Order>> GetHighValueOrdersAsync(decimal threshold)
    {
        return await _context.Orders
            .Where(x => x.Total > threshold)
            .ToListAsync();
    }

    public async Task<Order> CreateAsync(Order order)
    {
        order.CreatedDate = DateTime.Now;
        order.Status = "pending";
        
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<Order> UpdateAsync(Order order)
    {
        order.UpdatedDate = DateTime.Now;
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<bool> UpdateStatusAsync(string id, string newStatus)
    {
        var order = await GetByIdAsync(id);
        if (order == null) return false;

        order.Status = newStatus;
        order.UpdatedDate = DateTime.Now;
        
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var order = await GetByIdAsync(id);
        if (order == null) return false;

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<decimal> GetMonthlyRevenueAsync(int month, int year)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1);
        
        return await _context.Orders
            .Where(x => x.CreatedDate >= startDate && 
                       x.CreatedDate < endDate &&
                       (x.Status == "delivered" || x.Status == "shipped"))
            .SumAsync(x => x.Total);
    }

    public async Task<int> GetOrderCountByStatusAsync(string status)
    {
        return await _context.Orders
            .CountAsync(x => x.Status == status);
    }

    public async Task<IEnumerable<Order>> GetOrdersNeedingProcessingAsync()
    {
        var cutoffDate = DateTime.Now.AddHours(-24);
        return await _context.Orders
            .Where(x => x.Status == "pending" && x.CreatedDate < cutoffDate)
            .ToListAsync();
    }
}