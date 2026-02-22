namespace Dotnet9.Models
{
    public class CustomerMalls
    {
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }

        public int MallId { get; set; }
        public Mall? Mall { get; set; }

    }
}
