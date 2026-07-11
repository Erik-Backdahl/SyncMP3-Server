public interface ICompareRepository
{
    Task<string> GetSongPath(Guid songId, Guid networkId);
    Task<List<SongDTO>> GetAllUploadRequests(Guid networkID);
    Task<List<SongDTO>> GetAvailibleSongs(Guid networkId);
    Task TryAddRequestToSongs(List<Song> songs, Guid userId, Guid networkId);
    Task<List<Song>> GetNetworkSongs(Guid id);
    Task<List<SongDTO>> GetUserRequestedSongs(Guid userId);
    Task<List<Song>> GetUserCurrentDownloadedSongs(Guid id);
    Task SaveNewUserSongs(Guid userId, List<Song> newSongs);
    Task HandleNewDownloadedSong(DownloadedSong newDownloadedSong);
}