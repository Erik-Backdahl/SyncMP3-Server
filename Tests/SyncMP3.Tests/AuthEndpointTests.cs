using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

public class AuthEndpointTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;

    public AuthEndpointTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_ValidRequest_Returns201()
    {
        var body = JsonContent.Create(new { email = "a@b.com", password = "Pass123!" });

        var response = await _client.PostAsync("/api/auth/register", body);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}