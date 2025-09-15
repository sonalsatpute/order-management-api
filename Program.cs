using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using OrderManagementAPI.Data;
using OrderManagementAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Legacy database configurations - TECH DEBT: Hardcoded and mixed patterns
builder.Services.AddDbContext<PostgresContext>(options =>
    options.UseNpgsql("Host=localhost;Database=OrderManagement;Username=postgres;Password=password123"));

builder.Services.AddSingleton<IMongoClient>(s =>
    new MongoClient("mongodb://localhost:27017"));
builder.Services.AddSingleton<MongoContext>();

// TECH DEBT: Services registered as singletons inappropriately
builder.Services.AddSingleton<OrderService>();

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