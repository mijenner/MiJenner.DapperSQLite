using Microsoft.Extensions.Configuration;
using MiJenner.ServicesGeneric;

namespace DapperSQLite3TableConsole
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
            DbFileName = "products3t.sqlite3";
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

            var service = new ProductOrderService(ConnectionString);

            int rowsAffected;
            IEnumerable<Product>? productsEnumerable;
            IEnumerable<Order>? ordersEnumerable;
            IEnumerable<OrderItem>? orderItemsEnumerable;
            List<Product>? products;
            List<Order>? orders;
            List<OrderItem>? orderItems;

            // Slet alle data fra alle tre tabeller
            rowsAffected = await service.DeleteAllProductsAsync();
            rowsAffected = await service.DeleteAllOrdersAsync();
            rowsAffected = await service.DeleteAllOrderItemsAsync();

            // Opret produkter
            rowsAffected = await service.CreateProductAsync(new Product() { Name = "Appelsin", Price = 3.14, StockQuantity = 40 });
            rowsAffected = await service.CreateProductAsync(new Product() { Name = "Æble", Price = 5.14, StockQuantity = 140 });
            rowsAffected = await service.CreateProductAsync(new Product() { Name = "Banan", Price = 2.50, StockQuantity = 60 });

            // Læs alle produkter
            (rowsAffected, productsEnumerable) = await service.ReadAllProductsAsync();
            products = productsEnumerable?.ToList();

            // Opret ordrer
            rowsAffected = await service.CreateOrderAsync(new Order() { Customer = "John Doe", OrderDate = "2024-07-14 09:40" });
            rowsAffected = await service.CreateOrderAsync(new Order() { Customer = "Jane Doe", OrderDate = "2024-09-21 15:47" });

            // Læs alle ordrer
            (rowsAffected, ordersEnumerable) = await service.ReadAllOrdersAsync();
            orders = ordersEnumerable?.ToList();

            // Opret OrderItems (kobling mellem ordrer og produkter)
            rowsAffected = await service.CreateOrderItemAsync(new OrderItem() { OrderId = orders[0].OrderId, ProductId = products[0].ProductId, Quantity = 5 });
            rowsAffected = await service.CreateOrderItemAsync(new OrderItem() { OrderId = orders[0].OrderId, ProductId = products[1].ProductId, Quantity = 10 });
            rowsAffected = await service.CreateOrderItemAsync(new OrderItem() { OrderId = orders[1].OrderId, ProductId = products[2].ProductId, Quantity = 7 });

            // Læs alle OrderItems
            (rowsAffected, orderItemsEnumerable) = await service.ReadAllOrderItemsAsync();
            orderItems = orderItemsEnumerable?.ToList();

            // Forbered sletning: Læs første OrderItem og slet det
            var orderItemToDelete = orderItems?.FirstOrDefault();
            (rowsAffected, orderItemToDelete) = await service.ReadOrderItemByIdAsync(orderItemToDelete.OrderItemId);
            rowsAffected = await service.DeleteOrderItemAsync(orderItemToDelete.OrderItemId);

            // Læs OrderItems igen for at verificere sletning
            (rowsAffected, orderItemsEnumerable) = await service.ReadAllOrderItemsAsync();
            orderItems = orderItemsEnumerable?.ToList();

            // Opdater første produkt
            var productToUpdate = products?.FirstOrDefault();
            productToUpdate.Name = "Fisk";
            rowsAffected = await service.UpdateProductAsync(productToUpdate);

            // Læs produktet for at verificere opdatering
            (rowsAffected, productToUpdate) = await service.ReadProductByIdAsync(productToUpdate.ProductId);

        }
    }
}
