using System.Security.Cryptography;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

public class CompareRepository : ICompareRepository
{
    private readonly SyncMp3DbContext _dbContext;
    public CompareRepository(SyncMp3DbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<List<SongDTO>> GetAllUploadRequests(Guid networkID)
    {
        return await _dbContext.SongRequests
            .Where(s => s.NetworkId == networkID)
            .Select(s => new SongDTO
            {
                Id = s.SongNavigation.Id,
                Name = s.SongNavigation.Name,
                DurationSeconds = s.SongNavigation.DurationSeconds
            }).ToListAsync();
    }
    public async Task<List<SongDTO>> GetAvailibleSongs(Guid networkId)
    {
        return await _dbContext.DownloadedSongs
        .Where(n => n.NetworkId == networkId)
        .Select(n => new SongDTO
        {
            Id = n.SongNavigation.Id,
            Name = n.SongNavigation.Name,
            DurationSeconds = n.SongNavigation.DurationSeconds
        }).ToListAsync();
    }
    public async Task<List<SongDTO>> GetUserRequestedSongs(Guid userId)
    {
        return await _dbContext.SongRequests
            .Where(r => r.RequestedById == userId)
            .Select(r => new SongDTO
            {
                Id = r.SongNavigation.Id,
                Name = r.SongNavigation.Name,
                DurationSeconds = r.SongNavigation.DurationSeconds,
            })
            .ToListAsync();
    }
    public async Task TryAddRequestToSongs(List<Song> songs, Guid userId, Guid networkId)
    {
        var songIds = songs.Select(s => s.Id).ToHashSet();

        var alreadyDownloadedIds = await _dbContext.DomainUsers
            .Where(u => u.Id == userId)
            .SelectMany(u => u.LocalSongs)
            .Where(s => songIds.Contains(s.Id))
            .Select(s => s.Id)
            .ToHashSetAsync();

        var alreadyRequestedIds = await _dbContext.SongRequests
            .Where(r => r.RequestedById == userId && songIds.Contains(r.SongId))
            .Select(r => r.SongId)
            .ToHashSetAsync();

        var newRequests = songs
            .Where(s => !alreadyDownloadedIds.Contains(s.Id) && !alreadyRequestedIds.Contains(s.Id))
            .Select(s => new SongRequest
            {
                Id = Guid.NewGuid(),
                SongId = s.Id,
                NetworkId = networkId,
                RequestedById = userId
            });

        _dbContext.SongRequests.AddRange(newRequests);
        await _dbContext.SaveChangesAsync();
    }
    public async Task<List<Song>> GetNetworkSongs(Guid id)
    {
        var network = await _dbContext.Networks
            .Include(n => n.NetworkSongs)
            .SingleAsync(n => n.Id == id);

        return network.NetworkSongs.ToList();
    }
    public async Task<List<Song>> GetUserCurrentDownloadedSongs(Guid id)
    {
        var user = await _dbContext.DomainUsers
            .Include(u => u.LocalSongs)
            .SingleAsync(u => u.Id == id);

        return user.LocalSongs.ToList();
    }
    public async Task SaveNewUserSongs(Guid userId, List<Song> newSongs)
    {
        var transaction = await _dbContext.Database.BeginTransactionAsync();

        await _dbContext.Songs.AddRangeAsync(newSongs);

        var user = await _dbContext.DomainUsers
            .SingleAsync(u => u.Id == userId);

        user.LocalSongs.AddRange(newSongs);

        await _dbContext.SaveChangesAsync();
    }

    public async Task HandleNewDownloadedSong(DownloadedSong newDownloadedSong)
    {
        
        await _dbContext.DownloadedSongs.AddAsync(newDownloadedSong);

        var matchingRequests = await _dbContext.SongRequests.Where(s => s.SongId == newDownloadedSong.SongId).ToListAsync();
        _dbContext.SongRequests.RemoveRange(matchingRequests);

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            throw new ConflictException("Song already downloaded");
        }
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
        return ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2601 || sqlEx.Number == 2627);
    }
    public async Task<string> GetSongPath(Guid songId, Guid networkId)
    {
        var filePath = await _dbContext.DownloadedSongs
            .Where(s => s.SongId == songId && s.NetworkId == networkId)
            .Select(s => s.FilePath)
            .SingleOrDefaultAsync();

        if (filePath == null)
            throw new NotFoundException($"No Song found for SongId: {songId}");

        return filePath;

    }
}