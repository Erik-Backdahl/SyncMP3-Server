using Microsoft.EntityFrameworkCore;

public class UserRepository : IUserRepository
{
    private readonly SyncMp3DbContext _dbContext;
    public UserRepository(SyncMp3DbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<bool> UserNetworkMatchesUser(Guid userId, string? networkId)
    {
        Guid? parsedNetworkId = null;

        if (networkId != null)
        {
            if (!Guid.TryParse(networkId, out var parsed))
                throw new BadRequestException("Invalid NetworkId format");

            parsedNetworkId = parsed;
        }

        return await _dbContext.DomainUsers
            .AnyAsync(u => u.Id == userId && u.NetworkId == parsedNetworkId);
    }
    public async Task CreateUser(DomainUser newUser)
    {
        await _dbContext.DomainUsers.AddAsync(newUser);
        await _dbContext.SaveChangesAsync();
    }
    public async Task<bool> UserExists(Guid id)
    {
        return await _dbContext.DomainUsers.AnyAsync(u => u.Id == id);
    }
    public async Task<DomainUser> GetDomainUser(Guid id)
    {
        return await _dbContext.DomainUsers.SingleAsync(u => u.Id == id);
    }
}