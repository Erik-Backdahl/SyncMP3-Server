using System.ComponentModel.DataAnnotations;

public partial class Song
{
    public required Guid Id { get; set; }
    public required Guid NetworkId { get; set; }
    public required NetWork NetworkNavigation { get; set; }
    public List<DomainUser>? DownloadedBy { get; set; }
    public Guid? DownloadedSongId { get; set; }
    public DownloadedSong? DownloadedSongNavigation { get; set; }
    [MaxLength(300)]
    public string? Name { get; set; }
    public int DurationSeconds { get; set; }
}