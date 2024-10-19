using Dapper;
using System.Diagnostics;
using static SQLite.SQLite3;

namespace DapperSQLite1TableConsole
{
    public class ProductService : IProductService
    {
        private string _connectionString;
        private SqliteConnection _connection;

        public ProductService(string connectionString)
        {
            _connectionString = (String.IsNullOrEmpty(connectionString)) ? "database.sqlite3" : connectionString;
        }

        /// <summary>
        /// Initializes the database connection and ensures the Products table exists, only if it hasn't been initialized yet.
        /// </summary>
        private async Task InitAsync()
        {
            if (_connection is not null) return;

            _connection = new SqliteConnection(_connectionString);
            await _connection.OpenAsync();

            var createTableSql = @"
                CREATE TABLE IF NOT EXISTS Products (
                    ProductId INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Price REAL NOT NULL,
                    StockQuantity INTEGER NOT NULL
                );";
            await _connection.ExecuteAsync(createTableSql);

            Debug.WriteLine("*** Database initialized.");
        }

        // Create a product 
        public async Task<int> CreateProductAsync(Product product)
        {
            await InitAsync();

            var sql = "INSERT INTO Products (Name, Price, StockQuantity) VALUES (@Name, @Price, @StockQuantity)";
            return await _connection.ExecuteAsync(sql, new { product.Name, product.Price, product.StockQuantity } );
        }

        // Read all products 
        public async Task<(int, IEnumerable<Product>?)> ReadAllProductsAsync()
        {
            await InitAsync();

            var sql = "SELECT * FROM Products";

            var Result = await _connection.QueryAsync<Product>(sql);
            int rowsAffected = (Result.Count() == 0 || Result is null) ? 0 : Result.Count();
            return (rowsAffected, Result);
        }

        // Read a single product by ID
        public async Task<(int, Product?)> ReadProductByIdAsync(int productId)
        {
            await InitAsync();
            var sql = "SELECT * FROM Products WHERE ProductId = @ProductId";
            var Result = await _connection.QueryFirstOrDefaultAsync<Product>(sql, new { ProductId = productId });
            int rowsAffected = (Result is null) ? 0 : 1;
            return (rowsAffected, Result);
        }

        // Update a product 
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
    }
}
