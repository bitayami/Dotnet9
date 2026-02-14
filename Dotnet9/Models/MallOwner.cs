namespace Dotnet9.Models
{
    public class MallOwner
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactInfo { get; set; } = string.Empty;
        public int MallId { get; set; }
        public Mall? Mall { get; set; }

    }
}
