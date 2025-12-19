using System.Net;
using SyncMP3.Data;
using SyncMP3.Models;

class EasyEndpoints
{
    internal static async Task CreateNewUser(HttpListenerContext httpContext, SyncMp3Context dbContext)
    {
        User newUser = new()
        {
            UserUuid = Guid.NewGuid().ToString(),
            PublicKey = Guid.NewGuid().ToString() //not final obviously just to add something
        } ;

        dbContext.Users.Add(newUser);
        await dbContext.SaveChangesAsync();

        await Response.TextResponse(httpContext, "Created User");
    }
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