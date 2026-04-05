namespace Dotnet9.Models.DTO
{
    public class MallDocumentDto
    {
        public int Id { get; set; }
        public int MallId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
        public long FileSizeBytes { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public string DownloadUrl { get; set; } = string.Empty;

    }
}
