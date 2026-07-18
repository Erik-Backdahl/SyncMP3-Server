public class NetworkInfoDTO
{
    public required Guid NetworkId { get; set; }
    public required int TotalMembers { get; set; }
    public DateTime Created { get; set; }
}