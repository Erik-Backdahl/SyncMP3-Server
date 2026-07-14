using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

public class RemoveUserEndpointTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;
    private readonly ApiFactory _factory;

    public RemoveUserEndpointTests(ApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }
    [Fact]
    public async Task RemoveUser_RegularRemoval()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SyncMp3DbContext>();

        var scenario = await RemoveUserScenarios.RegularRemoval(db);

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", TestAuthHelper.GenerateToken(scenario.OwnerUser.Id));

        var request = new HttpRequestMessage(HttpMethod.Patch, "/api/network/remove-user");
        request.Headers.Add("X-Network-Id", scenario.OwnerUser.NetworkId.ToString());
        request.Headers.Add("X-Remove-Id", scenario.RemoveUser.Id.ToString());

        var response = await _client.SendAsync(request);
        await response.AssertStatusCode(HttpStatusCode.NoContent);
    }
    [Fact]
    public async Task RemoveUser_NotOwner()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SyncMp3DbContext>();

        var scenario = await RemoveUserScenarios.NotOwnerOfNetwork(db);

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", TestAuthHelper.GenerateToken(scenario.RegularUser.Id));

        var request = new HttpRequestMessage(HttpMethod.Patch, "/api/network/remove-user");
        request.Headers.Add("X-Network-Id", scenario.RegularUser.NetworkId.ToString());
        request.Headers.Add("X-Remove-Id", scenario.OwnerUser.Id.ToString());

        var response = await _client.SendAsync(request);
        await response.AssertStatusCode(HttpStatusCode.Unauthorized);
    }
    [Fact]
    public async Task RemoveUser_UserNotApartOfNetworkAlready()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SyncMp3DbContext>();

        var scenario = await RemoveUserScenarios.UserToBeRemovedIsNotInNetwork(db);

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", TestAuthHelper.GenerateToken(scenario.OwnerUser.Id));

        var request = new HttpRequestMessage(HttpMethod.Patch, "/api/network/remove-user");
        request.Headers.Add("X-Network-Id", scenario.OwnerUser.Id.ToString());
        request.Headers.Add("X-Remove-Id", scenario.UserApartOfOtherNetwork.Id.ToString());

        var response = await _client.SendAsync(request);
        await response.AssertStatusCode(HttpStatusCode.BadRequest);
    }

}