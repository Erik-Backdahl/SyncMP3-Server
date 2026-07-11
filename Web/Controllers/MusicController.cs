using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class MusicController : ControllerBase
{
    private readonly ICompareService _compareService;

    public MusicController(ICompareService compareService)
    {
        _compareService = compareService;
    }
    [HttpPost("compare")]
    public async Task<IActionResult> Compare(
        [FromBody] List<SongDTO> downloadedSongsDTO,
        [FromHeader(Name = "X-Network-Id")] Guid networkId)
    {
        var userId = Guid.Parse(User.FindFirst("sub")?.Value!);

        var requestedFromServer = await _compareService.AddRequestedSongs(downloadedSongsDTO, userId, networkId);
        var canDownloadSongs = await _compareService.GetAvailibleSongs(networkId);
        var requestedForUpload = await _compareService.GetUploadRequests(networkId);

        return Ok(new CompareResponseDTO
        {
            RequestedFromServer = requestedFromServer,
            CanDownloadSongs = canDownloadSongs,
            RequestedForUpload = requestedForUpload
        });
    }
    [HttpPost("upload")]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> Upload(
        [FromHeader(Name = "X-Song-Id")] Guid songId,
        [FromHeader(Name = "X-Song-Name")] string songNameEncoded,
        [FromHeader(Name = "X-Network-Id")] Guid networkId)
    {
        Request.EnableBuffering();
        Request.Body.Position = 0;

        string saveFolderPath = Path.Combine("C:/Repos/PROJECTS/SyncMP3/Music", networkId.ToString());

        if (!Directory.Exists(saveFolderPath))
            Directory.CreateDirectory(saveFolderPath);

        string songName = Uri.UnescapeDataString(songNameEncoded ?? throw new BadRequestException("No SongName found"));
        string savePath = Path.Combine(saveFolderPath, songName);
        try
        {
            byte[] songBytes;
            using var memoryStream = new MemoryStream();

            await Request.Body.CopyToAsync(memoryStream);
            songBytes = memoryStream.ToArray();

            var userId = Guid.Parse(User.FindFirst("sub")?.Value!);

            await System.IO.File.WriteAllBytesAsync(savePath, songBytes);

            await _compareService.HandleNewDownloadedSong(songId, networkId, userId, savePath);

            return Created();
        }
        catch
        {
            System.IO.File.Delete(savePath);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    [HttpGet("download")]
    public async Task<IActionResult> Download(
        [FromHeader(Name = "X-Song-Id")] Guid songId,
        [FromHeader(Name = "X-Network-Id")] Guid networkId)
    {
        string songPath = await _compareService.GetSongPath(songId, networkId);

        return PhysicalFile(songPath, "audio/mpeg", Path.GetFileName(songPath));
    }
}