using Dapper;
using DapperSQLite2TableConsole;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DapperSQLite2TableConsole; 

public class ProductService
{
    private string? _connectionString;
    private SqliteConnection? _connection;

    public ProductService(string connectionString)
    {
        _connectionString = string.IsNullOrEmpty(connectionString) ? "database.sqlite3" : connectionString;
    }

    private async Task InitAsync()
    {
        if (_connection is not null) return;

        _connection = new SqliteConnection(_connectionString);
        if (_connection is null) throw new Exception("_connection is null");

        await _connection.OpenAsync();

        var createCategoryTableSql = @"
            CREATE TABLE IF NOT EXISTS Categories (
                CategoryId INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL
            );";

        var createProductTableSql = @"
            CREATE TABLE IF NOT EXISTS Products (
                ProductId INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Price REAL NOT NULL,
                StockQuantity INTEGER NOT NULL,
                CategoryId INTEGER NOT NULL,
                FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId)
            );";

        await _connection.ExecuteAsync(createCategoryTableSql);
        await _connection.ExecuteAsync(createProductTableSql);

        Debug.WriteLine("*** Database initialized.");
    }

    // Create a new category
    public async Task<int> CreateCategoryAsync(Category category)
    {
        await InitAsync();

        var sql = "INSERT INTO Categories (Name) VALUES (@Name)";
        return await _connection.ExecuteAsync(sql, new { category.Name });
    }

    // Read all categories
    public async Task<(int, IEnumerable<Category>)> ReadAllCategoriesAsync()
    {
        await InitAsync();

        var sql = "SELECT * FROM Categories";
        var categories = await _connection.QueryAsync<Category>(sql);
        int rowsAffected = (categories is null) ? 0 : categories.Count();

        return (rowsAffected, categories);
    }

    // Create a new product
    public async Task<int> CreateProductAsync(Product product)
    {
        await InitAsync();

        var sql = "INSERT INTO Products (Name, Price, StockQuantity, CategoryId) VALUES (@Name, @Price, @StockQuantity, @CategoryId)";
        return await _connection.ExecuteAsync(sql, new { product.Name, product.Price, product.StockQuantity, product.CategoryId });
    }

    // Read all products
    public async Task<(int, IEnumerable<Product>)> ReadAllProductsAsync()
    {
        await InitAsync();

        var sql = "SELECT * FROM Products";
        var products = await _connection.QueryAsync<Product>(sql);
        int rowsAffected = (products is null) ? 0 : products.Count();

        return (rowsAffected, products);
    }

    // Read a product by ID
    public async Task<(int, Product?)> ReadProductByIdAsync(int productId)
    {
        await InitAsync();

        var sql = "SELECT * FROM Products WHERE ProductId = @ProductId";
        var product = await _connection.QueryFirstOrDefaultAsync<Product>(sql, new { ProductId = productId });
        int rowsAffected = (product is null) ? 0 : 1;

        return (rowsAffected, product);
    }

    // Update a product
    public async Task<int> UpdateProductAsync(Product product)
    {
        await InitAsync();

        var sql = "UPDATE Products SET Name = @Name, Price = @Price, StockQuantity = @StockQuantity, CategoryId = @CategoryId WHERE ProductId = @ProductId";
        return await _connection.ExecuteAsync(sql, new { product.ProductId, product.Name, product.Price, product.StockQuantity, product.CategoryId });
    }

    // Delete a product
    public async Task<int> DeleteProductAsync(int productId)
    {
        await InitAsync();

        var sql = "DELETE FROM Products WHERE ProductId = @ProductId";
        return await _connection.ExecuteAsync(sql, new { ProductId = productId });
    }

    // Delete all products
    public async Task<int> DeleteAllProductsAsync()
    {
        await InitAsync();

        var sql = "DELETE FROM Products";
        return await _connection.ExecuteAsync(sql);
    }

    // Delete all categories
    public async Task<int> DeleteAllCategoriesAsync()
    {
        await InitAsync();

        var sql = "DELETE FROM Categories";
        return await _connection.ExecuteAsync(sql);
    }
}
