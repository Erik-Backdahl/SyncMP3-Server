using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

public class NetworkKeyRepository : INetworkKeyRepository
{
    private readonly SyncMp3DbContext _dbContext;

    public NetworkKeyRepository(SyncMp3DbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<NetworkKey?> GetCurrentNetworkKey(Guid Id)
    {
        return await _dbContext.NetworkKeys.FirstOrDefaultAsync(n => n.NetworkId == Id);
    }
    public Task<NetworkKey> Create1HourKey(Guid Id)
    {
        return GenerateAndReplaceKeyAsync(Id, 1);
    }
    public Task<NetworkKey> Create24HourKey(Guid Id)
    {
        return GenerateAndReplaceKeyAsync(Id, 24);
    }
    private async Task<NetworkKey> GenerateAndReplaceKeyAsync(Guid networkId, int timeHours)
    {
        var existing = await _dbContext.NetworkKeys
            .Where(k => k.NetworkId == networkId)
            .ToListAsync();

        _dbContext.NetworkKeys.RemoveRange(existing);

        var key = new NetworkKey
        {
            Id = Guid.NewGuid(),
            Code = NetworkKeyGenerator.GenerateCode(),
            NetworkId = networkId,
            ExpiresAtUtc = DateTime.UtcNow.AddHours(timeHours)
        };

        _dbContext.NetworkKeys.Add(key);
        await _dbContext.SaveChangesAsync();
        return key;
    }

}