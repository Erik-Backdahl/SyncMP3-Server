using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;

public class CreateEndpointTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;
    private readonly ApiFactory _factory;

    public CreateEndpointTests(ApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }
    [Fact]
    public async Task Create_ReturnsCreated()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SyncMp3DbContext>();

        var scenario = await CreateScenarios.SuccessfulCreation(db);

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", TestAuthHelper.GenerateToken(scenario.User.Id));

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/network/create");

        var response = await _client.SendAsync(request);
        await response.AssertStatusCode(HttpStatusCode.Created);
    }
    [Fact]
    public async Task Create_AlreadyInNetwork()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SyncMp3DbContext>();

        var scenario = await CreateScenarios.AlreadyInNetwork(db);

        _client.DefaultRequestHeaders.Authorization =
           new AuthenticationHeaderValue("Bearer", TestAuthHelper.GenerateToken(scenario.User.Id));

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/network/create");

        var response = await _client.SendAsync(request);
        await response.AssertStatusCode(HttpStatusCode.BadRequest); //returns badrequest instead of conflict becasue the middleware notices that the network is not provided
    }
}