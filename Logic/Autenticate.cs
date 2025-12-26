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
        if (!dbContext.Users.Any(u => u.UserUuid == requestUuid))
        {
            await CreateEntries.NewUser(httpContext, dbContext);
            return false;
        }

        if (dbContext.Users.Where(u => u.NetworkGuid == requestGuid && u.UserUuid == requestUuid).Any())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    internal static bool VaildMusicRequest(HttpListenerContext httpContext, SyncMp3Context dbContext)
    {
        return true;
    }
}