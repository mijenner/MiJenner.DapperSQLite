using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperSQLite1TableConsole
{
    public interface IProductService
    {
        Task<int> CreateProductAsync(Product product);
        Task<(int, IEnumerable<Product>)> ReadAllProductsAsync();
        Task<(int, Product)> ReadProductByIdAsync(int productId);
        Task<int> UpdateProductAsync(Product product);
        Task<int> DeleteProductAsync(int productId);
        Task<int> DeleteAllProductsAsync(); 
    }
}
