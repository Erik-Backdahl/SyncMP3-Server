using System.Net;
using SyncMP3.Data;
using SyncMP3.Models;
class EasyEndpoints
{
    
    internal static async Task CreateResponse(HttpListenerResponse response)
    {

    }

    internal static async Task DefaultResponse(HttpListenerContext httpContext, SyncMp3Context dbContext)
    {
        await Response.TextResponse(httpContext, "Invalid", 400);
    }

    internal static async Task MenuResponse(HttpListenerResponse response)
    {
        throw new NotImplementedException();
    }

    internal static async Task PingResponse(HttpListenerResponse response)
    {
        
    }
}