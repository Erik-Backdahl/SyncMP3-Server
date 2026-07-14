public class NetworkService : INetworkService
{
    private readonly INetworkRepository _networkRepository;
    private readonly IUserRepository _userRepository;
    private readonly INetworkKeyRepository _networkKeyRepository;
    public NetworkService(
        INetworkRepository networkRepository,
        IUserRepository userRepository,
        INetworkKeyRepository networkKeyRepository)
    {
        _networkRepository = networkRepository;
        _userRepository = userRepository;
        _networkKeyRepository = networkKeyRepository;
    }

    public async Task<Network> TryJoinNetwork(Guid id, string code)
    {
        return await _networkRepository.JoinNetwork(id, code);
    }
    public async Task<NetworkCreateResponseDTO> CreateNewNetwork(Guid userId)
    {
        var user = await _userRepository.GetDomainUser(userId);

        if (user.NetworkId.HasValue && user.NetworkId != Guid.Empty)
            throw new ConflictException("User already part of a network");

        var network = await _networkRepository.CreateNewNetwork(user);

        var networkKey = await _networkKeyRepository.Create24HourKey(network.Id);

        return new NetworkCreateResponseDTO
        {
            NetworkId = network.Id,
            Code = networkKey.Code,
            Expires = networkKey.ExpiresAtUtc
        };
    }
    public async Task<NetworkKey> GenerateNewNetworkKey(Guid id)
    {
        var currentKey = await _networkKeyRepository.GetCurrentNetworkKey(id);

        if (currentKey == null || currentKey.IsExpired)
        {
            return await _networkKeyRepository.Create1HourKey(id);
        }
        if (currentKey.ExpiresAtUtc > DateTime.UtcNow.AddMinutes(58))
        {
            return currentKey;
        }
        else
        {
            return await _networkKeyRepository.Create1HourKey(id);
        }
    }
    public async Task RemoveUser(Guid userId, Guid removeUserId, Guid networkId)
    {
        var requesteeUser = await _userRepository.GetDomainUser(userId);
        var removeUser = await _userRepository.GetDomainUser(removeUserId);

        var network = await _networkRepository.GetNetwork(networkId);

        if (network.OwnerId != requesteeUser.Id)
            throw new UnauthorizedException("Not allowed to remove a user when not the owner of network");

        if (removeUser.NetworkId != networkId) // dont need to check for requesteeUser beacause that is checked in the middleware
            throw new BadRequestException("Requested user is not in the network");

        await _networkRepository.RemoveUserAndAssociatedSongs(networkId, removeUserId);
    }
    public async Task LeaveNetwork(Guid userId, Guid networkId)
    {
        var network = await _networkRepository.GetNetwork(networkId);
        if (network.OwnerId == userId)
            await _networkRepository.TransferTitle(networkId);


        await _networkRepository.RemoveUserAndAssociatedSongs(networkId, userId);
    }
    public async Task TransferTitle(Guid networkId, Guid currentOwnerId, Guid newOwnerId)
    {
        var network = await _networkRepository.GetNetwork(networkId);

        if (network.OwnerId != currentOwnerId)
            throw new UnauthorizedException("Cant transfer title when not the owner, if you have lost the device with the owner its best to create a new network");

        await _networkRepository.TransferTitle(networkId, newOwnerId);
    }
}