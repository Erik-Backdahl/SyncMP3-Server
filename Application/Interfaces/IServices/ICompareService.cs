public interface ICompareService
{
    Task<string> GetSongPath(Guid songID, Guid networkId);
    Task HandleNewDownloadedSong(Guid songId, Guid networkId, Guid userId, string filePath);
    Task<List<SongDTO>> AddRequestedSongs(List<SongDTO> userSongsDTO, Guid userId, Guid networkId);
    Task<List<SongDTO>> GetAvailibleSongs(Guid networkId);
    Task<List<SongDTO>> GetUploadRequests(Guid networkId);
}