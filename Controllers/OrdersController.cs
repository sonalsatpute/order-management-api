using Microsoft.AspNetCore.Mvc;
using OrderManagementAPI.Models;
using OrderManagementAPI.Repositories;
using OrderManagementAPI.Services;

namespace OrderManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly OrderService _orderService;
    
    public OrdersController(IOrderRepository orderRepository, OrderService orderService)
    {
        _orderRepository = orderRepository;
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrders(string? status = null, bool includeArchived = false)
    {
        try
        {
            IEnumerable<Order> orders;
            
            if (includeArchived)
            {
                var archivedOrders = await _orderRepository.GetArchivedOrdersAsync();
                var recentOrders = await _orderRepository.GetRecentOrdersAsync();
                orders = recentOrders.Concat(archivedOrders);
            }
            else
            {
                orders = string.IsNullOrEmpty(status) 
                    ? await _orderRepository.GetRecentOrdersAsync()
                    : await _orderRepository.GetByStatusAsync(status);
            }

            // TODO: Move business logic to a service
            foreach (var order in orders)
            {
                if (order.Total > 1000)
                {
                    order.Priority = "HIGH";
                }
                else if (order.Total > 500)
                {
                    order.Priority = "MEDIUM";
                }
                
                if (order.CreatedDate < DateTime.Now.AddHours(-24) && order.Status == "pending")
                {
                    order.Priority = "URGENT";
                }
            }
            
            return Ok(orders.OrderByDescending(x => x.CreatedDate));
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error retrieving orders: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder([FromBody] Order order)
    {
        if (order == null)
        {
            return BadRequest("Order is null");
        }
        
        try
        {
            // TODO: Add proper validation
            // TODO: Move business logic to service
            order.CreatedDate = DateTime.Now;
            order.Status = "pending";
            
            // Calculate totals
            decimal subtotal = order.Items.Sum(item => item.Quantity * item.UnitPrice);
            order.Subtotal = subtotal;
            order.Tax = subtotal * 0.08m; // TODO: Move to configuration
            order.Total = order.Subtotal + order.Tax;
            
            var createdOrder = await _orderRepository.CreateAsync(order);
            
            return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.Id ?? createdOrder.OrderId.ToString() }, createdOrder);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error creating order: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetOrderById(string id)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            
            return Ok(order);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error retrieving order: {ex.Message}");
        }
    }

    [HttpPatch("{id}/status")]
    public async Task<ActionResult> UpdateOrderStatus(string id, [FromBody] string newStatus)
    {
        var validStatuses = new[] { "pending", "processing", "shipped", "delivered", "cancelled" };
        if (!validStatuses.Contains(newStatus))
        {
            return BadRequest("Invalid status");
        }
        
        try
        {
            var success = await _orderRepository.UpdateStatusAsync(id, newStatus);
            if (!success)
            {
                return NotFound();
            }
            
            // TODO: Move notification logic to a service
            if (newStatus == "shipped")
            {
                var order = await _orderRepository.GetByIdAsync(id);
                if (order != null)
                {
                    Console.WriteLine($"Order {id} has been shipped to {order.CustomerEmail}");
                }
            }
            
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error updating order: {ex.Message}");
        }
    }
}