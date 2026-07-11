public partial class SongDTO
{
    public required Guid Id { get; set; }
    public string? Name { get; set; }
    public int DurationSeconds { get; set; }
    public Guid? OriginId { get; set; }
}