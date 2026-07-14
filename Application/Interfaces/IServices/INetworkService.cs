public interface INetworkService
{
    Task<Network> TryJoinNetwork(Guid id, string code);
    Task<NetworkCreateResponseDTO> CreateNewNetwork(Guid id);
    Task<NetworkKey> GenerateNewNetworkKey(Guid id);
    Task RemoveUser(Guid userId, Guid removeUserId, Guid networkId);
    Task LeaveNetwork(Guid userId, Guid networkId);
    Task TransferTitle(Guid networkId, Guid currentOwnerId, Guid newOwnerId);
}