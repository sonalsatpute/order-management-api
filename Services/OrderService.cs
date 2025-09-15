using OrderManagementAPI.Models;
using OrderManagementAPI.Repositories;

namespace OrderManagementAPI.Services;

// TODO: Create interface for this service
public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    
    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }
    
    public async Task<IEnumerable<Order>> GetRecentOrdersAsync(int days = 30)
    {
        return await _orderRepository.GetRecentOrdersAsync(days);
    }
    
    public async Task<decimal> CalculateMonthlyRevenueAsync(int month, int year)
    {
        return await _orderRepository.GetMonthlyRevenueAsync(month, year);
    }
    
    public async Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime start, DateTime end)
    {
        return await _orderRepository.GetOrdersByDateRangeAsync(start, end);
    }
    
    public async Task<bool> ProcessHighValueOrdersAsync()
    {
        try
        {
            const decimal HIGH_VALUE_THRESHOLD = 1000m;
            var highValueOrders = await _orderRepository.GetHighValueOrdersAsync(HIGH_VALUE_THRESHOLD);
            
            foreach (var order in highValueOrders)
            {
                Console.WriteLine($"Processing high value order: {order.Id}");
                
                if (order.Status == "pending")
                {
                    order.Priority = "HIGH";
                    await _orderRepository.UpdateAsync(order);
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing high value orders: {ex.Message}");
            return false;
        }
    }
    
    public async Task<bool> ValidateAndCreateOrderAsync(Order order)
    {
        // TODO: Move to dedicated validator
        if (string.IsNullOrEmpty(order.CustomerName) || 
            string.IsNullOrEmpty(order.CustomerEmail) ||
            !order.Items.Any())
        {
            return false;
        }
        
        try
        {
            // TODO: Move business logic to domain model
            order.CreatedDate = DateTime.Now;
            order.Status = "pending";
            
            decimal subtotal = order.Items.Sum(item => item.Quantity * item.UnitPrice);
            order.Subtotal = subtotal;
            order.Tax = subtotal * 0.08m; // TODO: Move to configuration
            order.Total = order.Subtotal + order.Tax;
            
            await _orderRepository.CreateAsync(order);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating order: {ex.Message}");
            return false;
        }
    }
}