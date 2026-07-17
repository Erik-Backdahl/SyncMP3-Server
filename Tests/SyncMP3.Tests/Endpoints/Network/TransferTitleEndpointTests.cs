using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

public class TransferTitleEndpointTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;
    private readonly ApiFactory _factory;

    public TransferTitleEndpointTests(ApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }
    [Fact]
    public async Task TransferTitle_RegularTransfer()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SyncMp3DbContext>();

        var scenario = await TransferTitleScenarios.RegularTransfer(db);

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", TestAuthHelper.GenerateToken(scenario.Owner.Id));

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/network/transfer-title");
        request.Headers.Add("X-Network-Id", scenario.Owner.NetworkId.ToString());
        request.Headers.Add("X-New-Owner-Id", scenario.NewOwner.Id.ToString());

        var response = await _client.SendAsync(request);
        await response.AssertStatusCode(HttpStatusCode.OK);
    }
    [Fact]
    public async Task TransferTitle_NotOwner()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SyncMp3DbContext>();

        var scenario = await TransferTitleScenarios.NotOwner(db);

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", TestAuthHelper.GenerateToken(scenario.RequestingUser.Id));

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/network/transfer-title");
        request.Headers.Add("X-Network-Id", scenario.RequestingUser.NetworkId.ToString());
        request.Headers.Add("X-New-Owner-Id", scenario.NotOwnerUser.Id.ToString());

        var response = await _client.SendAsync(request);
        await response.AssertStatusCode(HttpStatusCode.Unauthorized);
    }
}