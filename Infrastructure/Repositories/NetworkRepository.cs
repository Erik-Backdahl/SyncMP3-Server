using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;

public class NetworkRepository : INetworkRepository
{
    private readonly SyncMp3DbContext _dbContext;
    public NetworkRepository(SyncMp3DbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<Network> GetNetwork(Guid id)
    {
        var network = await _dbContext.Networks.FirstOrDefaultAsync(n => n.Id == id);

        if (network == null)
            throw new NotFoundException($"Network not found, id:{id}");

        return network;
    }
    public async Task<Network> GetNetworkAndSongs(Guid id)
    {
        var networkAndSongs = await _dbContext.Networks
            .Include(n => n.NetworkSongs)
            .Include(n => n.Users)
            .FirstOrDefaultAsync(n => n.Id == id);

        if(networkAndSongs == null)
            throw new NotFoundException("Network not found");

        return networkAndSongs;
    }
    public async Task<Network> JoinNetwork(Guid id, string code)
    {
        var user = await _dbContext.DomainUsers.SingleOrDefaultAsync(u => u.Id == id);
        if (user == null)
            throw new NotFoundException("User not found");

        if (user.NetworkId.GetValueOrDefault() != Guid.Empty)
            throw new ConflictException("User already part of a network");

        var key = await _dbContext.NetworkKeys
            .Include(k => k.NetworkNavigation)
            .ThenInclude(n => n.Users)
            .FirstOrDefaultAsync(n => n.Code == code);

        if (key is null || key.IsExpired)
            throw new NotFoundException("Invalid code");

        /* var network = _dbContext.Networks.Include(n => n.Users).SingleAsync(n => n.Id == key.NetworkId);
        //TODO CHECK IF MAX AMMOUNT OF USERS HAS BEEN REACHED
        if(network) */

        user.NetworkId = key.NetworkId;

        await _dbContext.SaveChangesAsync();

        return key.NetworkNavigation;
    }
    public async Task<Network> CreateNewNetwork(DomainUser user)
    {
        var newNetwork = new Network
        {
            Id = Guid.NewGuid(),
            OwnerId = user.Id
        };

        await _dbContext.Networks.AddAsync(newNetwork);
        user.NetworkId = newNetwork.Id;

        await _dbContext.SaveChangesAsync();

        return newNetwork;
    }
    public async Task RemoveUserAndAssociatedSongs(Guid networkId, Guid userId)
    {
        var songsToRemove = await _dbContext.Songs
        .Where(s => s.NetworkId == networkId)
        .Where(s => s.DownloadedBy.Count == 1 && s.DownloadedBy.Any(u => u.Id == userId))
        .ToListAsync();

        _dbContext.Songs.RemoveRange(songsToRemove);

        var user = await _dbContext.DomainUsers.SingleAsync(u => u.Id == userId);
        _dbContext.DomainUsers.Remove(user);

        await _dbContext.SaveChangesAsync();
    }
    public async Task TransferTitle(Guid networkId, Guid? newOwnerId = null)
    {
        var network = await _dbContext.Networks
            .Include(n => n.Users)
            .SingleOrDefaultAsync(n => n.Id == networkId);

        if (network == null)
            throw new NotFoundException("Network not found");

        var newOwner = network.Users.FirstOrDefault(u => u.Id != network.OwnerId);

        //on the app this "leave" button should be a "delete network" button if there is only one device left
        //basically this shouldnt really happen
        if (newOwner == null)
            throw new NotFoundException("No other members found please delete network instead");
        network.OwnerId = newOwner.Id;

        await _dbContext.SaveChangesAsync();
    }
}