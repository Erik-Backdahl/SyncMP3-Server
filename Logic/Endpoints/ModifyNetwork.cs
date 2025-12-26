using System.Net;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using SyncMP3.Data;
using SyncMP3.Models;

class ModifyNetwork
{
    internal static async Task AddUser(HttpListenerContext httpContext, SyncMp3Context dbContext)
    {
        try
        {
            var request = httpContext.Request;
            string? password = request.Headers.Get("NetworkKey");
            if (string.IsNullOrEmpty(password))
            {
                await Response.TextResponse(httpContext, "Invalid password", 400);
                return;
            }

            string uuid = request.Headers.Get("UUID")!;
            if (DbHelpers.CheckUserInNetwork(dbContext, uuid))
            {
                await Response.TextResponse(httpContext, "User already in a Network", 400);
                return;
            }

            if (!dbContext.NetworkPasswords.Any(p => p.Password == password))
            {
                await Response.TextResponse(httpContext, "Invalid password");
                return;
            }

            string guid = dbContext.NetworkPasswords.Where(p => p.Password == password).Select(p => p.NetworkGuid).Single();

            if (!await OpenSpotInNetwork(dbContext, guid))
            {
                await Response.TextResponse(httpContext, "Network Full", 400);
                return;
            }

            await ChangeUserNetwork(httpContext.Response, dbContext, uuid, guid);
            await IncreaseNetworkTotalUsers(dbContext, guid);

            await dbContext.SaveChangesAsync();

            await Response.TextResponse(httpContext, "Added to network");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    internal static async Task RemoveUser(HttpListenerContext httpContext, SyncMp3Context dbContext)
    {

    }

    internal static async Task RegisterMasterUser()
    {

    }
    internal static async Task TransferTitle()
    {

    }
    private static async Task<bool> OpenSpotInNetwork(SyncMp3Context dbContext, string guid)
    {
        if (dbContext.Networks.Any(n => n.NetworkGuid == guid && n.TotalUsers > 0))
            return false;
        else
            return true;

    }
    private static async Task ChangeUserNetwork(HttpListenerResponse response, SyncMp3Context dbContext, string uuid, string guid)
    {
        User user = dbContext.Users.Single(u => u.UserUuid == uuid);

        user.NetworkGuid = guid;

        response.AddHeader("GUID", guid);
    }
    private static async Task IncreaseNetworkTotalUsers(SyncMp3Context dbContext, string guid)
    {
        Network network = dbContext.Networks.Single(n => n.NetworkGuid == guid);

        network.TotalUsers++;
    }
    private static async Task DecreaseNetworkTotalUsers(SyncMp3Context dbContext, string guid)
    {
        Network network = dbContext.Networks.Single(n => n.NetworkGuid == guid);

        network.TotalUsers--;
    }
}