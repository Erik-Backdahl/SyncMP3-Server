using System.Net;

public static class HttpResponseAssertExtensions
{
    public static async Task<string> AssertStatusCode(
        this HttpResponseMessage response, HttpStatusCode expected)
    {
        var content = await response.Content.ReadAsStringAsync();

        Assert.True(
            response.StatusCode == expected,
            $"Expected status {expected} but got {response.StatusCode}.\nResponse body: {content}");

        return content; 
    }
}