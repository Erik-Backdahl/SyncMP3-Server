using System.ComponentModel.DataAnnotations;
public partial class DomainUser
{
    [Key]
    public required Guid Id { get; set; }
    public Guid? NetworkId { get; set; }
    public NetWork? NetworkNavigation { get; set; }
    public bool Premium { get; set; } = false;
    public DateTime? PremiumExpirationDate { get; set; }
    public List<Song>? CurrentSongs { get; set; }
    public List<RequestedSong>? RequestedSongs { get; set; }
}