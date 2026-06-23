using System.ComponentModel.DataAnnotations;
public partial class Song
{
    public Guid Id { get; set; }
    public Guid NetworkId { get; set; }
    public NetWork NetworkNavigation { get; set; } = null!;

    [MaxLength(300)]
    public string? Name { get; set; }
    public int DurationSeconds { get; set; }
    public List<SongRequest> SongRequests { get; set; } = [];
    public List<DomainUser> DownloadedBy { get; set; } = [];
    public DownloadedSong? DownloadedSong { get; set; }
}
