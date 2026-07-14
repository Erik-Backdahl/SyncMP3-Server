using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public partial class NetworkKey
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(8)]
    public string Code { get; set; } = null!;
    public Guid NetworkId { get; set; }
    public Network NetworkNavigation { get; set; } = null!;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAtUtc { get; set; }
    [NotMapped]
    public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;
}