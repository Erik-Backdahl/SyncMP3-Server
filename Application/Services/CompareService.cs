using System.Runtime.InteropServices;

public class CompareService : ICompareService
{
    private readonly ICompareRepository _compareRepository;
    public CompareService(ICompareRepository compareRepository)
    {
        _compareRepository = compareRepository;
    }

    public async Task<List<SongDTO>> AddRequestedSongs(List<SongDTO> userSongsDTO, Guid userId, Guid networkId)
    {
        var serverUserSongs = await _compareRepository.GetUserCurrentDownloadedSongs(userId);

        var serverSongIds = serverUserSongs.Select(s => s.Id).ToHashSet();

        var newlyDetectedSongs = userSongsDTO
            .Where(dto => !serverSongIds.Contains(dto.Id))
            .Select(dto => new Song
            {
                Id = dto.Id,
                NetworkId = networkId,
                Name = dto.Name,
                DurationSeconds = dto.DurationSeconds,
            })
            .ToList();

        if (newlyDetectedSongs.Count > 0)
            await _compareRepository.SaveNewUserSongs(userId, newlyDetectedSongs);

        var allUserSongIds = serverSongIds
            .Concat(newlyDetectedSongs.Select(s => s.Id))
            .ToHashSet();

        var networkSongs = await _compareRepository.GetNetworkSongs(networkId);

        var wantedSongs = networkSongs
            .Where(s => !allUserSongIds.Contains(s.Id))
            .ToList();

        await _compareRepository.TryAddRequestToSongs(wantedSongs, userId, networkId);

        return await _compareRepository.GetUserRequestedSongs(userId);
    }
    public async Task<List<SongDTO>> GetAvailibleSongs(Guid networkId)
    {
        return await _compareRepository.GetAvailibleSongs(networkId);
    }
    public async Task<List<SongDTO>> GetUploadRequests(Guid networkId)
    {
        return await _compareRepository.GetAllUploadRequests(networkId);
    }
    public async Task HandleNewDownloadedSong(Guid songId, Guid networkId, Guid userId, string filePath)
    {
        var downloadedSong = new DownloadedSong
        {
            SongId = songId,
            NetworkId = networkId,
            FilePath = filePath,
            UploadedBy = userId
        };
        
        await _compareRepository.HandleNewDownloadedSong(downloadedSong);
    }
    public Task<string> GetSongPath(Guid songId, Guid networkId)
    {
        return _compareRepository.GetSongPath(songId, networkId);
    }
}