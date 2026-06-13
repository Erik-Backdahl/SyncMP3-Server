using System.ComponentModel.DataAnnotations;

public partial class NetWork
{
    [Key]
    public required Guid Id { get; set; }
    public required Guid OwnerId { get; set; }
    public required User OwnerNavigation { get; set; }
    public required List<User> Users { get; set; }
    public List<Song>? NetworkSongs { get; set; }
    public List<DownloadedSong>? DownloadedSongs { get; set; }
}