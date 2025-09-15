using System.Linq.Expressions;
using Moq;
using OrderManagementAPI.Models;
using OrderManagementAPI.Repositories;
using Xunit;

namespace OrderManagementAPI.Tests.Repositories
{
    /// <summary>
    /// Unit tests for IRepository<T> interface contract and behavior
    /// </summary>
    public class IRepositoryTests
    {
        private readonly Mock<IRepository<Order>> _mockRepository;
        private readonly List<Order> _testOrders;

        public IRepositoryTests()
        {
            _mockRepository = new Mock<IRepository<Order>>();
            _testOrders = CreateTestOrders();
        }

        #region Create Operations Tests

        [Fact]
        public async Task CreateAsync_ShouldReturnCreatedEntity_WhenValidEntityProvided()
        {
            // Arrange
            var newOrder = new Order
            {
                CustomerName = "Test Customer",
                CustomerEmail = "test@example.com",
                Status = "pending",
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductName = "Test Product", Quantity = 1, UnitPrice = 10.00m }
                }
            };

            _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Order>()))
                .ReturnsAsync((Order order) =>
                {
                    order.Id = "test-id-123";
                    order.CreatedDate = DateTime.UtcNow;
                    return order;
                });

            // Act
            var result = await _mockRepository.Object.CreateAsync(newOrder);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test-id-123", result.Id);
            Assert.Equal("Test Customer", result.CustomerName);
            Assert.True(result.CreatedDate > DateTime.MinValue);
            _mockRepository.Verify(r => r.CreateAsync(newOrder), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowArgumentNullException_WhenNullEntityProvided()
        {
            // Arrange
            _mockRepository.Setup(r => r.CreateAsync(null))
                .ThrowsAsync(new ArgumentNullException(nameof(Order)));

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _mockRepository.Object.CreateAsync(null));
            _mockRepository.Verify(r => r.CreateAsync(null), Times.Once);
        }

        #endregion

        #region Read Operations Tests

        [Fact]
        public async Task GetByIdAsync_ShouldReturnEntity_WhenEntityExists()
        {
            // Arrange
            var expectedOrder = _testOrders.First();
            _mockRepository.Setup(r => r.GetByIdAsync("test-id-1"))
                .ReturnsAsync(expectedOrder);

            // Act
            var result = await _mockRepository.Object.GetByIdAsync("test-id-1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedOrder.Id, result.Id);
            Assert.Equal(expectedOrder.CustomerName, result.CustomerName);
            _mockRepository.Verify(r => r.GetByIdAsync("test-id-1"), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllEntities()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAllAsync())
                .ReturnsAsync(_testOrders);

            // Act
            var result = await _mockRepository.Object.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_testOrders.Count, result.Count());
            _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        #endregion

        #region Helper Methods

        private List<Order> CreateTestOrders()
        {
            return new List<Order>
            {
                new Order
                {
                    Id = "test-id-1",
                    OrderId = 1001,
                    CustomerName = "John Doe",
                    CustomerEmail = "john@example.com",
                    Status = "pending",
                    Priority = "NORMAL",
                    Subtotal = 100.00m,
                    Tax = 8.00m,
                    Total = 108.00m,
                    CreatedDate = DateTime.UtcNow.AddDays(-1),
                    Items = new List<OrderItem>
                    {
                        new OrderItem { ProductName = "Product A", Quantity = 2, UnitPrice = 50.00m }
                    }
                }
            };
        }

        #endregion
    }
}