using MongoDB.Driver;
using OrderManagementAPI.Models;

namespace OrderManagementAPI.Data;

// TECH DEBT: No interface, tightly coupled to concrete implementation
public class MongoContext
{
    private readonly IMongoDatabase _database;
    
    // TECH DEBT: Hardcoded database name
    public MongoContext(IMongoClient client)
    {
        _database = client.GetDatabase("OrderManagementDB");
    }
    
    // TECH DEBT: Public properties expose implementation details
    public IMongoCollection<Order> Orders => _database.GetCollection<Order>("orders");
    
    // TECH DEBT: No connection health checking
    // TECH DEBT: No retry policies
    // TECH DEBT: No logging
}