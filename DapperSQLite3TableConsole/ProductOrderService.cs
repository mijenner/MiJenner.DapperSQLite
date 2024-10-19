using Dapper;
using Microsoft.Data.Sqlite;
using System.Diagnostics;


namespace DapperSQLite3TableConsole
{
    public class ProductOrderService
    {
        private readonly string _connectionString;
        private SqliteConnection _connection;

        public ProductOrderService(string connectionString)
        {
            _connectionString = string.IsNullOrEmpty(connectionString) ? "database.sqlite3" : connectionString;
        }

        private async Task InitAsync()
        {
            if (_connection != null) return;

            _connection = new SqliteConnection(_connectionString);
            if (_connection is null) throw new Exception("_connection is null");
            await _connection.OpenAsync();

            string createTablesSql = @"
            CREATE TABLE IF NOT EXISTS Products (
                ProductId INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Price REAL NOT NULL,
                StockQuantity INTEGER NOT NULL
            );
            CREATE TABLE IF NOT EXISTS Orders (
                OrderId INTEGER PRIMARY KEY AUTOINCREMENT,
                Customer TEXT NOT NULL,
                OrderDate TEXT NOT NULL
            );
            CREATE TABLE IF NOT EXISTS OrderItems (
                OrderItemId INTEGER PRIMARY KEY AUTOINCREMENT,
                OrderId INTEGER NOT NULL,
                ProductId INTEGER NOT NULL,
                Quantity INTEGER NOT NULL,
                FOREIGN KEY (OrderId) REFERENCES Orders(OrderId) ON DELETE CASCADE,
                FOREIGN KEY (ProductId) REFERENCES Products(ProductId) ON DELETE CASCADE
            );";
            await _connection.ExecuteAsync(createTablesSql);
        }

        #region Product Methods
        public async Task<int> CreateProductAsync(Product product)
        {
            await InitAsync();
            var sql = "INSERT INTO Products (Name, Price, StockQuantity) VALUES (@Name, @Price, @StockQuantity)";
            return await _connection.ExecuteAsync(sql, new { product.Name, product.Price, product.StockQuantity });
        }

        public async Task<(int, IEnumerable<Product>)> ReadAllProductsAsync()
        {
            await InitAsync();
            var sql = "SELECT * FROM Products";
            var result = await _connection.QueryAsync<Product>(sql);
            return (result.Count(), result);
        }

        public async Task<(int, Product)> ReadProductByIdAsync(int productId)
        {
            await InitAsync();
            var sql = "SELECT * FROM Products WHERE ProductId = @ProductId";
            var result = await _connection.QueryFirstOrDefaultAsync<Product>(sql, new { ProductId = productId });
            return (result != null ? 1 : 0, result);
        }

        public async Task<int> UpdateProductAsync(Product product)
        {
            await InitAsync();
            var sql = "UPDATE Products SET Name = @Name, Price = @Price, StockQuantity = @StockQuantity WHERE ProductId = @ProductId";
            return await _connection.ExecuteAsync(sql, new { product.ProductId, product.Name, product.Price, product.StockQuantity });
        }

        public async Task<int> DeleteProductAsync(int productId)
        {
            await InitAsync();
            var sql = "DELETE FROM Products WHERE ProductId = @ProductId";
            return await _connection.ExecuteAsync(sql, new { ProductId = productId });
        }

        public async Task<int> DeleteAllProductsAsync()
        {
            await InitAsync();
            var sql = "DELETE FROM Products";
            return await _connection.ExecuteAsync(sql);
        }
        #endregion

        #region Order Methods
        public async Task<int> CreateOrderAsync(Order order)
        {
            await InitAsync();
            var sql = "INSERT INTO Orders (Customer, OrderDate) VALUES (@Customer, @OrderDate)";
            return await _connection.ExecuteAsync(sql, new { order.Customer, order.OrderDate });
        }

        public async Task<(int, IEnumerable<Order>)> ReadAllOrdersAsync()
        {
            await InitAsync();
            var sql = "SELECT * FROM Orders";
            var result = await _connection.QueryAsync<Order>(sql);
            return (result.Count(), result);
        }

        public async Task<(int, Order)> ReadOrderByIdAsync(int orderId)
        {
            await InitAsync();
            var sql = "SELECT * FROM Orders WHERE OrderId = @OrderId";
            var result = await _connection.QueryFirstOrDefaultAsync<Order>(sql, new { OrderId = orderId });
            return (result != null ? 1 : 0, result);
        }

        public async Task<int> UpdateOrderAsync(Order order)
        {
            await InitAsync();
            var sql = "UPDATE Orders SET Customer = @Customer, OrderDate = @OrderDate WHERE OrderId = @OrderId";
            return await _connection.ExecuteAsync(sql, new { order.OrderId, order.Customer, order.OrderDate });
        }

        public async Task<int> DeleteOrderAsync(int orderId)
        {
            await InitAsync();
            var sql = "DELETE FROM Orders WHERE OrderId = @OrderId";
            return await _connection.ExecuteAsync(sql, new { OrderId = orderId });
        }

        public async Task<int> DeleteAllOrdersAsync()
        {
            await InitAsync();
            var sql = "DELETE FROM Orders";
            return await _connection.ExecuteAsync(sql);
        }
        #endregion

        #region OrderItem Methods
        public async Task<int> CreateOrderItemAsync(OrderItem orderItem)
        {
            await InitAsync();
            var sql = "INSERT INTO OrderItems (OrderId, ProductId, Quantity) VALUES (@OrderId, @ProductId, @Quantity)";
            return await _connection.ExecuteAsync(sql, new { orderItem.OrderId, orderItem.ProductId, orderItem.Quantity });
        }

        public async Task<(int, IEnumerable<OrderItem>)> ReadAllOrderItemsAsync()
        {
            await InitAsync();
            var sql = "SELECT * FROM OrderItems";
            var result = await _connection.QueryAsync<OrderItem>(sql);
            return (result.Count(), result);
        }

        public async Task<(int, OrderItem)> ReadOrderItemByIdAsync(int orderItemId)
        {
            await InitAsync();
            var sql = "SELECT * FROM OrderItems WHERE OrderItemId = @OrderItemId";
            var result = await _connection.QueryFirstOrDefaultAsync<OrderItem>(sql, new { OrderItemId = orderItemId });
            return (result != null ? 1 : 0, result);
        }

        public async Task<int> UpdateOrderItemAsync(OrderItem orderItem)
        {
            await InitAsync();
            var sql = "UPDATE OrderItems SET OrderId = @OrderId, ProductId = @ProductId, Quantity = @Quantity WHERE OrderItemId = @OrderItemId";
            return await _connection.ExecuteAsync(sql, new { orderItem.OrderItemId, orderItem.OrderId, orderItem.ProductId, orderItem.Quantity });
        }

        public async Task<int> DeleteOrderItemAsync(int orderItemId)
        {
            await InitAsync();
            var sql = "DELETE FROM OrderItems WHERE OrderItemId = @OrderItemId";
            return await _connection.ExecuteAsync(sql, new { OrderItemId = orderItemId });
        }

        public async Task<int> DeleteAllOrderItemsAsync()
        {
            await InitAsync();
            var sql = "DELETE FROM OrderItems";
            return await _connection.ExecuteAsync(sql);
        }
        #endregion
    }
}

