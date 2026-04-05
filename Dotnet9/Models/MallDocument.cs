namespace Dotnet9.Models
{
    public class MallDocument
    {
        public int Id { get; set; }
        public int MallId { get; set; }
        public Mall? Mall { get; set; }

        public string FileName { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
        public long FileSizeBytes { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public string SavedFileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
    }
}
