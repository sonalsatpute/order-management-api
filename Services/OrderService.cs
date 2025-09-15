using MongoDB.Driver;
using OrderManagementAPI.Data;
using OrderManagementAPI.Models;

namespace OrderManagementAPI.Services;

// TECH DEBT: No interface, concrete implementation only
// TECH DEBT: God class with too many responsibilities
public class OrderService
{
    private readonly MongoContext _mongoContext;
    private readonly PostgresContext _postgresContext;
    
    // TECH DEBT: Injecting data contexts directly instead of repositories
    public OrderService(MongoContext mongoContext, PostgresContext postgresContext)
    {
        _mongoContext = mongoContext;
        _postgresContext = postgresContext;
    }
    
    // TECH DEBT: Not async, blocking operations
    public List<Order> GetRecentOrders(int days = 30)
    {
        var cutoffDate = DateTime.Now.AddDays(-days);
        var orders = new List<Order>();
        
        // TECH DEBT: Duplicate query logic
        var mongoOrders = _mongoContext.Orders
            .Find(x => x.CreatedDate >= cutoffDate)
            .ToList();
        orders.AddRange(mongoOrders);
        
        var pgOrders = _postgresContext.Orders
            .Where(x => x.CreatedDate >= cutoffDate)
            .ToList();
        orders.AddRange(pgOrders);
        
        return orders;
    }
    
    // TECH DEBT: Business logic mixed with data access
    public decimal CalculateMonthlyRevenue(int month, int year)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1);
        
        // TECH DEBT: N+1 query potential
        var orders = GetOrdersByDateRange(startDate, endDate);
        
        // TECH DEBT: Business calculation in service instead of domain
        decimal total = 0;
        foreach (var order in orders)
        {
            if (order.Status == "delivered" || order.Status == "shipped")
            {
                total += order.Total;
            }
        }
        
        return total;
    }
    
    // TECH DEBT: Private method doing too much
    private List<Order> GetOrdersByDateRange(DateTime start, DateTime end)
    {
        var orders = new List<Order>();
        
        // TECH DEBT: Inefficient - loading all orders then filtering
        var allMongoOrders = _mongoContext.Orders.Find(FilterDefinition<Order>.Empty).ToList();
        orders.AddRange(allMongoOrders.Where(x => x.CreatedDate >= start && x.CreatedDate < end));
        
        var allPgOrders = _postgresContext.Orders.ToList();
        orders.AddRange(allPgOrders.Where(x => x.CreatedDate >= start && x.CreatedDate < end));
        
        return orders;
    }
    
    // TECH DEBT: Void method, no return value for success/failure
    public void ProcessHighValueOrders()
    {
        // TECH DEBT: Magic number
        var highValueOrders = GetRecentOrders(7).Where(x => x.Total > 1000).ToList();
        
        foreach (var order in highValueOrders)
        {
            // TECH DEBT: Side effects, no error handling
            Console.WriteLine($"Processing high value order: {order.Id}");
            
            // TECH DEBT: Hardcoded business rules
            if (order.Status == "pending")
            {
                order.Priority = "HIGH";
                order.UpdatedDate = DateTime.Now;
                
                // TECH DEBT: Update logic duplicated from controller
                try
                {
                    if (!string.IsNullOrEmpty(order.Id))
                    {
                        _mongoContext.Orders.ReplaceOne(x => x.Id == order.Id, order);
                    }
                    else
                    {
                        _postgresContext.SaveChanges();
                    }
                }
                catch
                {
                    // TECH DEBT: Swallowing exceptions
                }
            }
        }
    }
    
    // TECH DEBT: Method doing validation, business logic, and data access
    public bool ValidateAndCreateOrder(Order order)
    {
        // TECH DEBT: Basic validation in service instead of dedicated validator
        if (string.IsNullOrEmpty(order.CustomerName) || 
            string.IsNullOrEmpty(order.CustomerEmail) ||
            !order.Items.Any())
        {
            return false;
        }
        
        // TECH DEBT: Business logic in service
        order.CreatedDate = DateTime.Now;
        order.Status = "pending";
        
        // TECH DEBT: Manual calculation
        decimal subtotal = order.Items.Sum(item => item.Quantity * item.UnitPrice);
        order.Subtotal = subtotal;
        order.Tax = subtotal * 0.08m; // TECH DEBT: Hardcoded tax rate
        order.Total = order.Subtotal + order.Tax;
        
        // TECH DEBT: Storage decision in service layer
        try
        {
            if (order.Total > 500)
            {
                _postgresContext.Orders.Add(order);
                _postgresContext.SaveChanges();
            }
            else
            {
                _mongoContext.Orders.InsertOne(order);
            }
            return true;
        }
        catch
        {
            // TECH DEBT: Swallowing exceptions, no logging
            return false;
        }
    }
}