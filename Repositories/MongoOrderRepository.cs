using MongoDB.Driver;
using OrderManagementAPI.Data;
using OrderManagementAPI.Models;

namespace OrderManagementAPI.Repositories;

/// <summary>
/// MongoDB implementation of the Order repository
/// Handles orders stored in MongoDB (typically low-value orders)
/// </summary>
public class MongoOrderRepository : IOrderRepository
{
    private readonly MongoContext _context;
    private readonly IMongoCollection<Order> _orders;

    public MongoOrderRepository(MongoContext context)
    {
        _context = context;
        _orders = context.Orders;
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await _orders.Find(FilterDefinition<Order>.Empty).ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetByStatusAsync(string status)
    {
        var filter = Builders<Order>.Filter.Eq(x => x.Status, status);
        return await _orders.Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetRecentOrdersAsync(int days = 30)
    {
        var cutoffDate = DateTime.Now.AddDays(-days);
        var filter = Builders<Order>.Filter.Gte(x => x.CreatedDate, cutoffDate);
        return await _orders.Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var filter = Builders<Order>.Filter.And(
            Builders<Order>.Filter.Gte(x => x.CreatedDate, startDate),
            Builders<Order>.Filter.Lt(x => x.CreatedDate, endDate)
        );
        return await _orders.Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetArchivedOrdersAsync(int archiveDays = 90)
    {
        var archiveDate = DateTime.Now.AddDays(-archiveDays);
        var filter = Builders<Order>.Filter.Lt(x => x.CreatedDate, archiveDate);
        return await _orders.Find(filter).ToListAsync();
    }

    public async Task<Order?> GetByIdAsync(string id)
    {
        var filter = Builders<Order>.Filter.Eq(x => x.Id, id);
        return await _orders.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Order>> GetHighValueOrdersAsync(decimal threshold)
    {
        var filter = Builders<Order>.Filter.Gt(x => x.Total, threshold);
        return await _orders.Find(filter).ToListAsync();
    }

    public async Task<Order> CreateAsync(Order order)
    {
        order.CreatedDate = DateTime.Now;
        order.Status = "pending";
        await _orders.InsertOneAsync(order);
        return order;
    }

    public async Task<Order> UpdateAsync(Order order)
    {
        order.UpdatedDate = DateTime.Now;
        var filter = Builders<Order>.Filter.Eq(x => x.Id, order.Id);
        await _orders.ReplaceOneAsync(filter, order);
        return order;
    }

    public async Task<bool> UpdateStatusAsync(string id, string newStatus)
    {
        var filter = Builders<Order>.Filter.Eq(x => x.Id, id);
        var update = Builders<Order>.Update
            .Set(x => x.Status, newStatus)
            .Set(x => x.UpdatedDate, DateTime.Now);
        
        var result = await _orders.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var filter = Builders<Order>.Filter.Eq(x => x.Id, id);
        var result = await _orders.DeleteOneAsync(filter);
        return result.DeletedCount > 0;
    }

    public async Task<decimal> GetMonthlyRevenueAsync(int month, int year)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1);
        
        var filter = Builders<Order>.Filter.And(
            Builders<Order>.Filter.Gte(x => x.CreatedDate, startDate),
            Builders<Order>.Filter.Lt(x => x.CreatedDate, endDate),
            Builders<Order>.Filter.In(x => x.Status, new[] { "delivered", "shipped" })
        );

        var orders = await _orders.Find(filter).ToListAsync();
        return orders.Sum(x => x.Total);
    }

    public async Task<int> GetOrderCountByStatusAsync(string status)
    {
        var filter = Builders<Order>.Filter.Eq(x => x.Status, status);
        return (int)await _orders.CountDocumentsAsync(filter);
    }

    public async Task<IEnumerable<Order>> GetOrdersNeedingProcessingAsync()
    {
        var cutoffDate = DateTime.Now.AddHours(-24);
        var filter = Builders<Order>.Filter.And(
            Builders<Order>.Filter.Eq(x => x.Status, "pending"),
            Builders<Order>.Filter.Lt(x => x.CreatedDate, cutoffDate)
        );
        return await _orders.Find(filter).ToListAsync();
    }
}