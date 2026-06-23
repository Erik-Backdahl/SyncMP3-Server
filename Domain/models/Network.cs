using System.ComponentModel.DataAnnotations;

public partial class NetWork
{
    [Key]
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public DomainUser OwnerNavigation { get; set; } = null!;
    public List<DomainUser> Users { get; set; } = [];
    public List<Song> NetworkSongs { get; set; } = [];
    public List<DownloadedSong> DownloadedSongs { get; set; } = [];
}