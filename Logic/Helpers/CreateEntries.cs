using SyncMP3.Data;
using SyncMP3.Models;
using System.Net;
class CreateEntries
{
    public static async Task NewUser(HttpListenerContext httpContext, SyncMp3Context dbContext)
    {
        User newUser = new()
        {
            UserUuid = Guid.NewGuid().ToString()
        };

        dbContext.Users.Add(newUser);
        await dbContext.SaveChangesAsync();

        await Response.TextResponse(httpContext, "Created User");
    }
    public static async Task NewNetwork(HttpListenerContext httpContext, SyncMp3Context dbContext)
    {
        
    }
    public static async Task NewCurrentNetworkRegisteredSongs(HttpListenerContext httpContext, SyncMp3Context dbContext)
    {
        
    }
    public static async Task NewCurrentUserRegisteredSongs(HttpListenerContext httpContext, SyncMp3Context dbContext)
    {
        
    }
}