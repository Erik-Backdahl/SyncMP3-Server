using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

public class UploadEndpointTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;
    private readonly ApiFactory _factory;

    public UploadEndpointTests(ApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }
    [Fact]
    public async Task Upload_ReturnsSuccess()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SyncMp3DbContext>();

        var scenario = await UploadScenarios.RegularUpload(db);

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", TestAuthHelper.GenerateToken(scenario.HasSong.Id));

        var fileBytes = new byte[] { 0x49, 0x44, 0x33, 0x03, 0x00, 0x00, 0x00 };
        var fileContent = new ByteArrayContent(fileBytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("audio/mpeg");

        using var multipart = new MultipartFormDataContent
        {
            { fileContent, "file", "test-song.mp3" }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/music/upload")
        {
            Content = multipart
        };
        request.Headers.Add("X-Song-Id", scenario.UploadSong.Id.ToString());
        request.Headers.Add("X-Song-Name", Uri.EscapeDataString(scenario.UploadSong.Name!));
        request.Headers.Add("X-Network-Id", scenario.Network.Id.ToString());


        var response = await _client.SendAsync(request);
        await response.AssertStatusCode(HttpStatusCode.Created);

        string networkPath = Path.Combine("C:/Repos/PROJECTS/SyncMP3/Music", scenario.Network.Id.ToString());
        string filePath = Path.Combine(networkPath, scenario.UploadSong.Name!);

        Assert.True(File.Exists(filePath));
        File.Delete(filePath);
        Directory.Delete(networkPath);
    }
    [Fact]
    public async Task Upload_ReturnsAlreadyUploaded()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SyncMp3DbContext>();

        var scenario = await UploadScenarios.DuplicateSongUpload(db);

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", TestAuthHelper.GenerateToken(scenario.HasSong.Id));

        var fileBytes = new byte[] { 0x49, 0x44, 0x33, 0x03, 0x00, 0x00, 0x00 };
        var fileContent = new ByteArrayContent(fileBytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("audio/mpeg");

        using var multipart = new MultipartFormDataContent
        {
            { fileContent, "file", "test-song.mp3" }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/music/upload")
        {
            Content = multipart
        };
        request.Headers.Add("X-Song-Id", scenario.UploadSong.Id.ToString());
        request.Headers.Add("X-Song-Name", Uri.EscapeDataString(scenario.UploadSong.Name!));
        request.Headers.Add("X-Network-Id", scenario.Network.Id.ToString());


        var id = scenario.UploadSong.Id.ToString();
        var response = await _client.SendAsync(request);
        await response.AssertStatusCode(HttpStatusCode.InternalServerError);

        string networkPath = Path.Combine("C:/Repos/PROJECTS/SyncMP3/Music", scenario.Network.Id.ToString());
        string filePath = Path.Combine(networkPath, scenario.UploadSong.Name!);

        Assert.False(File.Exists(filePath));
        File.Delete(filePath);
        Directory.Delete(networkPath);
    }
}