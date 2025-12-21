using System.Net;
using Microsoft.IdentityModel.Tokens;
using SyncMP3.Data;
using SyncMP3.Models;

class Authenticate
{
    internal static async Task<bool> RequestValid(HttpListenerContext httpContext, SyncMp3Context dbContext)
    {
        HttpListenerRequest request = httpContext.Request;

        string? requestUuid = request.Headers.Get("UUID");
        string? requestGuid = request.Headers.Get("GUID");

        if (string.IsNullOrEmpty(requestUuid) || !Guid.TryParse(requestUuid, out _))
            return false;
        if (requestGuid == null)
            return false;

        User? currentUser = dbContext.Users.FirstOrDefault(u => u.UserUuid == requestUuid);
        if(currentUser == null)
        {
            await CreateEntries.NewUser(httpContext, dbContext);
            return false;
        }

        //a request needs a guid header. if not in a network it should be an empty string
        //if its the users first time pinging the server they never have a guid header so the tryparse below is moved down insted of being above
    
        if(Guid.TryParse(requestGuid, out _))
            return false;
        if(currentUser.NetworkGuid == requestGuid || currentUser.NetworkGuid == null)
            return true;
        else
            return false;
    }

    internal static bool VaildMusicRequest(HttpListenerContext httpContext, SyncMp3Context dbContext)
    {
        return true;
    }
    
}