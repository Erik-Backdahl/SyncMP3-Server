using System.Net.NetworkInformation;

public class SongRequest
{
    public Guid Id { get; set; }
    public Guid SongId { get; set; }
    public Song SongNavigation { get; set; } = null!;
    public Guid RequestedById { get; set; }
    public DomainUser RequestedByNavigation { get; set; } = null!;
    public Guid NetworkId { get; set; }
    public Network NetworkNavigation { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}