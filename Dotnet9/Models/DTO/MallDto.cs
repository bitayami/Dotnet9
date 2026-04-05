namespace Dotnet9.Models.DTO
{
    public class MallDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int Floors { get; set; }
        public List<IFormFile>? Documents { get; set; }

    }
}
