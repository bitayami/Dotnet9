namespace Dotnet9.Models
{
    public class Shop
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int MallId { get; set; }
        public Mall Mall { get; set; }
    }
}
