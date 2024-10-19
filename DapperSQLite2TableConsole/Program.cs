using Microsoft.Extensions.Configuration;
using MiJenner.ServicesGeneric;

namespace DapperSQLite2TableConsole
{
    internal class Program
    {
        private static IAppSettingsService settingsService;
        private static string DbFileName;
        private static string DbFilePath;
        private static string CompanyName;
        private static string AppName;
        private static string ConnectionString;
        private static string FolderPath;
        private static string FilePath;
        private static string FileName;

        static async Task Main(string[] args)
        {
            // General 
            CompanyName = "companyname";
            AppName = "appname";
            FileName = "appsettings.json";
            (FolderPath, FilePath) = HelpersGeneric.SetFileAndFolderPaths(AppSettingsFolderPolicy.PolicyAppDataLocal, CompanyName, AppName, FileName);

            // App specifics 
            DbFileName = "productswcat.sqlite3";
            DbFilePath = Path.Combine(FolderPath, DbFileName);
            ConnectionString = $"Data Source={DbFilePath}";

            settingsService = new AppSettingsServiceConsole(AppSettingsFolderPolicy.PolicyAppDataLocal, CompanyName, AppName, FileName);

            if (!settingsService.ConfigExists)
            {
                settingsService.CreateSettingsFile(new { DbFileName = DbFileName, ConnectionString = ConnectionString });
            }

            IConfiguration configuration = settingsService.GetConfiguration();
            HelpersGeneric.WriteConfiguration(configuration);

            Console.WriteLine($"Database will be located at: {FolderPath}");

            ProductService productService = new ProductService(ConnectionString);

            int rowsAffected;
            List<Product>? products;
            IEnumerable<Product>? productsEnumerable;
            List<Category>? categories;
            IEnumerable<Category>? categoriesEnumerable;

            // First, delete all products and categories
            rowsAffected = await productService.DeleteAllProductsAsync();
            rowsAffected = await productService.DeleteAllCategoriesAsync();

            // Create categories
            rowsAffected = await productService.CreateCategoryAsync(new Category() { Name = "Fruits" });
            rowsAffected = await productService.CreateCategoryAsync(new Category() { Name = "Vegetables" });
            rowsAffected = await productService.CreateCategoryAsync(new Category() { Name = "Dairy" });

            // Read all categories
            (rowsAffected, categoriesEnumerable) = await productService.ReadAllCategoriesAsync();
            categories = categoriesEnumerable?.ToList();

            // Create products
            if (categories?.Count > 0)
            {
                var fruitCategoryId = categories[0].CategoryId; // Assuming "Fruits" is the first category
                rowsAffected = await productService.CreateProductAsync(new Product() { Name = "Appelsin", Price = 3.14, StockQuantity = 40, CategoryId = fruitCategoryId });
                rowsAffected = await productService.CreateProductAsync(new Product() { Name = "Æble", Price = 5.14, StockQuantity = 140, CategoryId = fruitCategoryId });
                rowsAffected = await productService.CreateProductAsync(new Product() { Name = "Banan", Price = 2.50, StockQuantity = 60, CategoryId = fruitCategoryId });
            }

            // Read all products
            (rowsAffected, productsEnumerable) = await productService.ReadAllProductsAsync();
            products = productsEnumerable?.ToList();

            // Prepare delete
            var productToDelete = products?.FirstOrDefault();

            // Read (to test service)
            (rowsAffected, productToDelete) = await productService.ReadProductByIdAsync(productToDelete.ProductId);

            // Delete
            if (productToDelete != null)
            {
                rowsAffected = await productService.DeleteProductAsync(productToDelete.ProductId);
            }

            // Update
            (rowsAffected, productsEnumerable) = await productService.ReadAllProductsAsync();
            products = productsEnumerable?.ToList();
            var productToUpdate = products?.FirstOrDefault();
            if (productToUpdate != null)
            {
                productToUpdate.Name = "Fisk"; // Change product name to "Fisk"
                rowsAffected = await productService.UpdateProductAsync(productToUpdate);
            }

            // Optionally read all products again to confirm update
            (rowsAffected, productsEnumerable) = await productService.ReadAllProductsAsync();
            products = productsEnumerable?.ToList();

        }
    }
}
