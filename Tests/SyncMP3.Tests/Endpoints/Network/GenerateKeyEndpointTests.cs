using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Azure;
using Microsoft.Extensions.DependencyInjection;

public class GenerateKeyEndpointTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;
    private readonly ApiFactory _factory;

    public GenerateKeyEndpointTests(ApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }
    [Fact]
    public async Task GenerateKey_Success()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SyncMp3DbContext>();

        var scenario = await GenerateKeyScenarios.ExpiredKey(db);

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", TestAuthHelper.GenerateToken(scenario.User.Id));

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/network/generate-key");
        request.Headers.Add("X-Network-Id", scenario.User.NetworkId.ToString());

        var response = await _client.SendAsync(request);
        await response.AssertStatusCode(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<NetworkCreateResponseDTO>();

        Assert.NotNull(body);
    }
    [Fact]
    public async Task GenerateKey_TooRecent()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SyncMp3DbContext>();

        var scenario = await GenerateKeyScenarios.TooRecentKey(db);

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", TestAuthHelper.GenerateToken(scenario.User.Id));

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/network/generate-key");
        request.Headers.Add("X-Network-Id", scenario.User.NetworkId.ToString());
        
        var response = await _client.SendAsync(request);
        await response.AssertStatusCode(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<NetworkCreateResponseDTO>();

        Assert.NotNull(body);
        Assert.Equal(scenario.NetworkKey.Code, body.Code);
    }



}
