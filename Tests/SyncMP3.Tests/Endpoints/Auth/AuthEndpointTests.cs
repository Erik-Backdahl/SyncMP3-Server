using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;

public class AuthEndpointTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;
    private readonly ApiFactory _factory;

    public AuthEndpointTests(ApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateAnonymousUser_ReturnsOkWithToken()
    {
        var response = await _client.PostAsync("/api/auth/anonymous", content: null);
        
        await response.AssertStatusCode(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<AnonymousAuthResponse>();
        Assert.NotNull(body);

        Assert.False(string.IsNullOrWhiteSpace(body.Token));
    }

    [Fact]
    public async Task CreateAnonymousUser_PersistsUserInDatabase()
    {
        var response = await _client.PostAsync("/api/auth/anonymous", content: null);

        await response.AssertStatusCode(HttpStatusCode.OK);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SyncMp3DbContext>();

        Assert.Single(db.DomainUsers);
    }
}