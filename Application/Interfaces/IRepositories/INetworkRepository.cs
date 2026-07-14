public interface INetworkRepository
{
    Task<Network> GetNetwork(Guid id);
    Task<Network> JoinNetwork(Guid id, string code);
    Task<Network> CreateNewNetwork(DomainUser user);
    Task RemoveUserAndAssociatedSongs(Guid networkId, Guid userId);
    Task TransferTitle(Guid networkId, Guid? newOwnerId = null);
}