namespace Dotnet9.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        //public int? ShopId { get; set; }
        public List<Shop>? Shops { get; set; }
        public List<Mall>? Malls { get; set; }
        public ICollection<CustomerMalls>? CustomerMalls { get; set; }
    }
}
