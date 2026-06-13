using System.ComponentModel.DataAnnotations;
public partial class DownloadedSong
{
    public int Id { get; set; }
    public required Guid SongId { get; set; }
    public required Song SongNavigation { get; set; }
    public required Guid NetworkId { get; set; }
    public required NetWork NetWorkNavigation { get; set; }
    [MaxLength(500)]
    public required string FilePath { get; set; }
    [MaxLength(36)]
    public required Guid OriginId { get; set; }
}