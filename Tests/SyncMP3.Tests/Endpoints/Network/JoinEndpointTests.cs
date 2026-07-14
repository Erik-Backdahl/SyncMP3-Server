using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

public class JoinEndpointTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;
    private readonly ApiFactory _factory;

    public JoinEndpointTests(ApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }
    [Fact]
    public async Task Join_RegularJoin()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SyncMp3DbContext>();

        var scenario = await JoinScenarios.SuccessJoin(db);

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", TestAuthHelper.GenerateToken(scenario.User.Id));

        var request = new HttpRequestMessage(HttpMethod.Patch, "/api/network/join");
        request.Headers.Add("X-Code", scenario.NetworkKey.Code);

        var response = await _client.SendAsync(request);
        await response.AssertStatusCode(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<NetworkInfoDTO>();

        Assert.NotNull(body);

        Assert.Equal(body.NetworkId.ToString(), scenario.NetworkId);
    }
    [Fact]
    public async Task Join_ExpiredCode()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SyncMp3DbContext>();

        var scenario = await JoinScenarios.OldCode(db);

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", TestAuthHelper.GenerateToken(scenario.User.Id));

        var request = new HttpRequestMessage(HttpMethod.Patch, "/api/network/join");
        request.Headers.Add("X-Code", scenario.NetworkKey.Code);

        var response = await _client.SendAsync(request);
        await response.AssertStatusCode(HttpStatusCode.NotFound);
    }
    [Fact]
    public async Task Join_AlreadyInOtherNetwork()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SyncMp3DbContext>();

        var scenario = await JoinScenarios.UserAlreadyInNetwork(db);

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", TestAuthHelper.GenerateToken(scenario.UserAlreadyInOtherNetwork.Id));

        var request = new HttpRequestMessage(HttpMethod.Patch, "/api/network/join");
        request.Headers.Add("X-Code", scenario.NetworkKey.Code);

        var response = await _client.SendAsync(request);
        await response.AssertStatusCode(HttpStatusCode.BadRequest);

    }
}
