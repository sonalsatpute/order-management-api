using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using OrderManagementAPI.Data;
using OrderManagementAPI.Repositories;
using OrderManagementAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database configurations - TODO: Move to configuration
builder.Services.AddDbContext<PostgresContext>(options =>
    options.UseNpgsql("Host=localhost;Database=OrderManagement;Username=postgres;Password=password123"));

builder.Services.AddSingleton<IMongoClient>(s =>
    new MongoClient("mongodb://localhost:27017"));
builder.Services.AddSingleton<MongoContext>();

// Repository pattern implementation
builder.Services.AddScoped<MongoOrderRepository>();
builder.Services.AddScoped<PostgresOrderRepository>();
builder.Services.AddScoped<IOrderRepository, CompositeOrderRepository>();

// Services - Properly scoped
builder.Services.AddScoped<OrderService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();