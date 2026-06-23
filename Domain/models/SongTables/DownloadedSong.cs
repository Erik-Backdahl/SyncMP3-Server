using System.ComponentModel.DataAnnotations;

public partial class DownloadedSong
{
    public int Id { get; set; }
    public Guid SongId { get; set; }
    public Song SongNavigation { get; set; } = null!;
    public Guid NetworkId { get; set; }
    public NetWork NetWorkNavigation { get; set; } = null!;
    [MaxLength(500)]
    public required string FilePath { get; set; }
    public Guid UploadedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}