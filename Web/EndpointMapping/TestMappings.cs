public static class TestMapping
{
    public static void ConfigureEndpoints(WebApplication app)
    {
        app.MapGet("/test", async (HttpContext httpContext, ITestService testService) =>
        {

        });
    }
}