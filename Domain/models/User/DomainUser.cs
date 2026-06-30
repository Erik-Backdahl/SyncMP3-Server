using System.ComponentModel.DataAnnotations;
public partial class DomainUser
{
    public Guid Id { get; set; }
    public Guid? NetworkId { get; set; }
    public Network? NetworkNavigation { get; set; }
    public bool Premium { get; set; } = false;
    public DateTime? PremiumExpirationDate { get; set; }
    public List<SongRequest> SongRequests { get; set; } = [];
    public List<Song> DownloadedSongs { get; set; } = [];
}