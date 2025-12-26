using SyncMP3.Data;
using SyncMP3.Models;
using System.Net;
using System.Net.Http.Headers;
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
}