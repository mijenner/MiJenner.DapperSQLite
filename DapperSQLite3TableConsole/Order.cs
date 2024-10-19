namespace DapperSQLite3TableConsole
{
    public class Order
    {
        public int OrderId { get; set; }
        public string Customer { get; set; }
        public string OrderDate { get; set; }
        // because SQLite doens't have a real date time data type. 
        // "yyyy-MM-dd HH:mm"
    }
}
