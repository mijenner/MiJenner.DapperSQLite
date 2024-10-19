using Microsoft.Extensions.Configuration;
using MiJenner.ServicesGeneric;

namespace DapperSQLite1TableConsole
{
    public class Program
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
            DbFileName = "products.sqlite3"; 
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

            // Delete all 
            rowsAffected = await productService.DeleteAllProductsAsync(); 
            // Create 
            rowsAffected = await productService.CreateProductAsync(new Product() { Name = "Appelsin", Price = 3.14, StockQuantity = 40 });
            rowsAffected = await productService.CreateProductAsync(new Product() { Name = "Æble", Price = 5.14, StockQuantity = 140 });
            rowsAffected = await productService.CreateProductAsync(new Product() { Name = "Gryn", Price = 113.14, StockQuantity = 20 });
            rowsAffected = await productService.CreateProductAsync(new Product() { Name = "Banan", Price = 2.50, StockQuantity = 60 });
            rowsAffected = await productService.CreateProductAsync(new Product() { Name = "Kirsebær", Price = 10.00, StockQuantity = 30 });
            rowsAffected = await productService.CreateProductAsync(new Product() { Name = "Mango", Price = 12.50, StockQuantity = 25 });
            rowsAffected = await productService.CreateProductAsync(new Product() { Name = "Pære", Price = 4.75, StockQuantity = 50 });
            rowsAffected = await productService.CreateProductAsync(new Product() { Name = "Druer", Price = 7.30, StockQuantity = 80 });
            rowsAffected = await productService.CreateProductAsync(new Product() { Name = "Ananas", Price = 15.00, StockQuantity = 10 });
            // Read all 
            (rowsAffected, productsEnumerable) = await productService.ReadAllProductsAsync(); 
            products = productsEnumerable?.ToList();
            // Prepare delete: 
            var productToDelete = products?.FirstOrDefault();
            // Read (to test service): 
            (rowsAffected, productToDelete) = await productService.ReadProductByIdAsync(productToDelete.ProductId);
            // Delete 
            rowsAffected = await productService.DeleteProductAsync(productToDelete.ProductId);
            (rowsAffected, productsEnumerable) = await productService.ReadAllProductsAsync();
            products = productsEnumerable?.ToList();
            // Update 
            var productToUpdate = products?.FirstOrDefault();
            productToUpdate.Name = "Fisk"; 
            rowsAffected = await productService.UpdateProductAsync(productToUpdate); 

        }
    }
}
