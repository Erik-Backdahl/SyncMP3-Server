public interface INetworkKeyRepository
{
    Task<NetworkKey?> GetCurrentNetworkKey(Guid Id);
    Task<NetworkKey> Create24HourKey(Guid Id);
    Task<NetworkKey> Create1HourKey(Guid Id);
}