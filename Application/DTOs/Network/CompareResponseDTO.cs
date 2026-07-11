public class CompareResponseDTO
{
    public List<SongDTO> RequestedFromServer { get; set; } = [];
    public List<SongDTO> CanDownloadSongs { get; set; } = [];
    public List<SongDTO> RequestedForUpload { get; set; } = [];
}