namespace Dotnet9.Models
{
    public class Mall
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int Floors { get; set; }
        public MallOwner MallOwner { get; set; }
    }
}
