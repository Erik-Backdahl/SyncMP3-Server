using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

public class DownloadEndpointTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;
    private readonly ApiFactory _factory;

    public DownloadEndpointTests(ApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }
    [Fact]
    public async Task Download_ReturnsOk()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SyncMp3DbContext>();

        var scenario = await DownloadScenario.AvailibleSong(db);

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", TestAuthHelper.GenerateToken(scenario.User.Id));


        var parentDir = Path.GetDirectoryName(scenario.SongToDownload.FilePath)!;
        Directory.CreateDirectory(parentDir);

        var fileBytes = new byte[] { 0x49, 0x44, 0x33, 0x03, 0x00, 0x00, 0x00 }; // same fake MP3 bytes as upload test
        await File.WriteAllBytesAsync(scenario.SongToDownload.FilePath, fileBytes);

        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/music/download");
            request.Headers.Add("X-Network-Id", scenario.User.NetworkId.ToString());
            request.Headers.Add("X-Song-Id", scenario.SongToDownload.SongId.ToString());

            var response = await _client.SendAsync(request);
            await response.AssertStatusCode(HttpStatusCode.OK);

            Assert.Equal("audio/mpeg", response.Content.Headers.ContentType?.MediaType);

            Assert.NotNull(response.Content.Headers.ContentDisposition);
            Assert.Equal(
                Path.GetFileName(scenario.SongToDownload.FilePath),
                response.Content.Headers.ContentDisposition!.FileName?.Trim('"'));

            var actualBytes = await response.Content.ReadAsByteArrayAsync();
            Assert.Equal(fileBytes, actualBytes);
        }
        finally
        {
            // cleanup regardless of pass/fail
            if (File.Exists(scenario.SongToDownload.FilePath))
                File.Delete(scenario.SongToDownload.FilePath);
            if (Directory.Exists(parentDir) && !Directory.EnumerateFileSystemEntries(parentDir).Any())
                Directory.Delete(parentDir);
        }
    }
    [Fact]
    public async Task Download_ReturnsNotFound()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SyncMp3DbContext>();

        var scenario = await DownloadScenario.UnavailibleSong(db);

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", TestAuthHelper.GenerateToken(scenario.User.Id));

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/music/download");
        request.Headers.Add("X-Network-Id", scenario.User.NetworkId.ToString());
        request.Headers.Add("X-Song-Id", Guid.NewGuid().ToString());

        var response = await _client.SendAsync(request);
        await response.AssertStatusCode(HttpStatusCode.NotFound);
    }
}