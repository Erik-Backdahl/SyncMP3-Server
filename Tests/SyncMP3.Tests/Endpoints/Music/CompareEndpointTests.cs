using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

public class CompareEndpointTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;
    private readonly ApiFactory _factory;

    public CompareEndpointTests(ApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }
    [Fact]
    public async Task Compare_ReturnsNewCanDownloadSongs()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SyncMp3DbContext>();

        var scenario = await CompareScenarios.NetworkWithUndownloadedSongs(db, memberCount: 3, songCount: 5);

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", TestAuthHelper.GenerateToken(scenario.Owner.Id));

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/music/compare")
        {
            Content = JsonContent.Create(new List<SongDTO>())  // owner has nothing locally
        };
        request.Headers.Add("X-Network-Id", scenario.Network.Id.ToString());

        var response = await _client.SendAsync(request);

        await response.AssertStatusCode(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<CompareResponseDTO>();
        Assert.NotNull(body);
        Assert.Equal(scenario.DownloadedSongs.Count, body!.CanDownloadSongs.Count);
    }
    [Fact]
    public async Task Compare_ReturnsRequestForUpload()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SyncMp3DbContext>();

        var scenario = await CompareScenarios.NetWorkWithUploadRequests(db);


        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", TestAuthHelper.GenerateToken(scenario.UserThatCanUpload.Id));

        var songsToCompare = scenario.UserThatCanUpload.LocalSongs
            .Select(song => new SongDTO { Id = song.Id, DurationSeconds = song.DurationSeconds })
            .ToList();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/music/compare")
        {
            Content = JsonContent.Create(songsToCompare)
        };
        request.Headers.Add("X-Network-Id", scenario.Network.Id.ToString());

        var response = await _client.SendAsync(request);

        await response.AssertStatusCode(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<CompareResponseDTO>();

        Assert.NotNull(body);
        Assert.Equal(scenario.Network.SongRequests.Count, body!.RequestedForUpload.Count);
    }
}