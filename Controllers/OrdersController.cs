using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using OrderManagementAPI.Data;
using OrderManagementAPI.Models;
using OrderManagementAPI.Services;

namespace OrderManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    // TECH DEBT: Too many dependencies injected directly
    private readonly MongoContext _mongoContext;
    private readonly PostgresContext _postgresContext;
    private readonly OrderService _orderService;
    
    public OrdersController(MongoContext mongoContext, PostgresContext postgresContext, OrderService orderService)
    {
        _mongoContext = mongoContext;
        _postgresContext = postgresContext;
        _orderService = orderService;
    }

    // TECH DEBT: Fat method with multiple responsibilities
    [HttpGet]
    public ActionResult<IEnumerable<Order>> GetOrders(string? status = null, bool includeArchived = false)
    {
        try
        {
            var orders = new List<Order>();
            
            // TECH DEBT: Business logic mixed with data access
            // TECH DEBT: No async/await
            
            // Get from MongoDB (recent orders)
            var mongoFilter = Builders<Order>.Filter.Empty;
            if (!string.IsNullOrEmpty(status))
            {
                mongoFilter = Builders<Order>.Filter.Eq(x => x.Status, status);
            }
            
            var mongoOrders = _mongoContext.Orders.Find(mongoFilter).ToList();
            orders.AddRange(mongoOrders);
            
            // TECH DEBT: Duplicate logic for Postgres
            if (includeArchived)
            {
                var pgOrders = _postgresContext.Orders.Where(x => x.CreatedDate < DateTime.Now.AddDays(-90)).ToList();
                if (!string.IsNullOrEmpty(status))
                {
                    pgOrders = pgOrders.Where(x => x.Status == status).ToList();
                }
                orders.AddRange(pgOrders);
            }
            
            // TECH DEBT: Business logic in controller
            foreach (var order in orders)
            {
                // TECH DEBT: Magic numbers
                if (order.Total > 1000)
                {
                    order.Priority = "HIGH";
                }
                else if (order.Total > 500)
                {
                    order.Priority = "MEDIUM";
                }
                
                // TECH DEBT: More business logic
                if (order.CreatedDate < DateTime.Now.AddHours(-24) && order.Status == "pending")
                {
                    order.Priority = "URGENT";
                }
            }
            
            // TECH DEBT: In-memory sorting of potentially large dataset
            return Ok(orders.OrderByDescending(x => x.CreatedDate).ToList());
        }
        catch (Exception ex)
        {
            // TECH DEBT: Poor error handling
            return BadRequest("Something went wrong: " + ex.Message);
        }
    }

    // TECH DEBT: Another fat method
    [HttpPost]
    public ActionResult<Order> CreateOrder([FromBody] Order order)
    {
        // TECH DEBT: No validation
        if (order == null)
        {
            return BadRequest("Order is null");
        }
        
        try
        {
            // TECH DEBT: Business logic in controller
            order.CreatedDate = DateTime.Now;
            order.Status = "pending";
            
            // TECH DEBT: Manual calculation instead of using domain model
            decimal subtotal = 0;
            foreach (var item in order.Items)
            {
                subtotal += item.Quantity * item.UnitPrice;
            }
            order.Subtotal = subtotal;
            
            // TECH DEBT: Magic numbers for tax calculation
            order.Tax = subtotal * 0.08m; // 8% tax hardcoded
            order.Total = order.Subtotal + order.Tax;
            
            // TECH DEBT: Deciding storage based on order value in controller
            if (order.Total > 500)
            {
                // Store high-value orders in Postgres
                _postgresContext.Orders.Add(order);
                _postgresContext.SaveChanges();
            }
            else
            {
                // Store low-value orders in MongoDB
                _mongoContext.Orders.InsertOne(order);
            }
            
            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
        }
        catch (Exception ex)
        {
            // TECH DEBT: Poor error handling
            return StatusCode(500, "Error creating order: " + ex.Message);
        }
    }

    // TECH DEBT: Yet another method with mixed concerns
    [HttpGet("{id}")]
    public ActionResult<Order> GetOrderById(string id)
    {
        try
        {
            // TECH DEBT: Try both databases - inefficient
            var mongoOrder = _mongoContext.Orders.Find(x => x.Id == id).FirstOrDefault();
            if (mongoOrder != null)
            {
                return Ok(mongoOrder);
            }
            
            // TECH DEBT: Different ID types for different databases
            if (int.TryParse(id, out int orderId))
            {
                var pgOrder = _postgresContext.Orders.FirstOrDefault(x => x.OrderId == orderId);
                if (pgOrder != null)
                {
                    return Ok(pgOrder);
                }
            }
            
            return NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error retrieving order: " + ex.Message);
        }
    }

    // TECH DEBT: Business logic in controller
    [HttpPatch("{id}/status")]
    public ActionResult UpdateOrderStatus(string id, [FromBody] string newStatus)
    {
        // TECH DEBT: No validation of status values
        var validStatuses = new[] { "pending", "processing", "shipped", "delivered", "cancelled" };
        if (!validStatuses.Contains(newStatus))
        {
            return BadRequest("Invalid status");
        }
        
        try
        {
            // TECH DEBT: Same inefficient lookup pattern
            var mongoOrder = _mongoContext.Orders.Find(x => x.Id == id).FirstOrDefault();
            if (mongoOrder != null)
            {
                mongoOrder.Status = newStatus;
                mongoOrder.UpdatedDate = DateTime.Now;
                
                // TECH DEBT: More business logic
                if (newStatus == "shipped")
                {
                    // Send notification - but this is hardcoded
                    Console.WriteLine($"Order {id} has been shipped to {mongoOrder.CustomerEmail}");
                }
                
                _mongoContext.Orders.ReplaceOne(x => x.Id == id, mongoOrder);
                return Ok(mongoOrder);
            }
            
            if (int.TryParse(id, out int orderId))
            {
                var pgOrder = _postgresContext.Orders.FirstOrDefault(x => x.OrderId == orderId);
                if (pgOrder != null)
                {
                    pgOrder.Status = newStatus;
                    pgOrder.UpdatedDate = DateTime.Now;
                    
                    // TECH DEBT: Duplicate notification logic
                    if (newStatus == "shipped")
                    {
                        Console.WriteLine($"Order {orderId} has been shipped to {pgOrder.CustomerEmail}");
                    }
                    
                    _postgresContext.SaveChanges();
                    return Ok(pgOrder);
                }
            }
            
            return NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error updating order: " + ex.Message);
        }
    }
}