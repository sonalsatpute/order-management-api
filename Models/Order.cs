using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace OrderManagementAPI.Models;

// TECH DEBT: Anemic domain model - no business logic, just data container
public class Order
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    [Key] // TECH DEBT: Mixed annotations for different databases
    public int OrderId { get; set; }
    
    // TECH DEBT: No validation attributes
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    
    // TECH DEBT: Primitive obsession
    public string Status { get; set; } = "pending"; // Should be enum
    
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }
    
    // TECH DEBT: No validation, public setters everywhere
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime? UpdatedDate { get; set; }
    
    // TECH DEBT: String for priority instead of enum
    public string Priority { get; set; } = "NORMAL";
    
    // TECH DEBT: List without proper encapsulation
    public List<OrderItem> Items { get; set; } = new();
    
    // TECH DEBT: Magic numbers in business logic scattered throughout
    public bool IsHighValue => Total > 1000; // Magic number
    public bool IsUrgent => Priority == "HIGH" || Priority == "URGENT"; // String comparison
}

// TECH DEBT: Another anemic model
public class OrderItem
{
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal => Quantity * UnitPrice; // At least this is calculated
}