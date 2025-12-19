using System.Net;

class Endpoints
{
    internal static async Task CreateResponse(HttpListenerResponse response)
    {
        
    }

    internal static async Task DefaultResponse(HttpListenerResponse response)
    {
        await Response.TextResponse(response, "Invalid", 400);
    }

    internal static async Task MenuResponse(HttpListenerResponse response)
    {
        throw new NotImplementedException();
    }

    internal static async Task PingResponse(HttpListenerResponse response)
    {
        await Response.TextResponse(response, "connected");
    }
}