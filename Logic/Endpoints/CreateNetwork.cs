using Microsoft.EntityFrameworkCore.Storage;
using SyncMP3.Data;
using SyncMP3.Models;
using System.Net;
class CreateNetwork
{
    internal static async Task CreateNewNetwork(HttpListenerContext httpContext, SyncMp3Context dbContext)
    {
        var request = httpContext.Request;

        string uuid = request.Headers.Get("UUID")!;
        if (request.Headers.Get("GUID") != "")
        {
            await Response.TextResponse(httpContext, "User already in a network", 400);
            return;
        }

        string networkGuid = Guid.NewGuid().ToString();

        // Start a transaction
        using IDbContextTransaction transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            User currentUser = dbContext.Users.FirstOrDefault(u => u.UserUuid == uuid)!;
            currentUser.NetworkGuid = networkGuid;

            var network = new Network()
            {
                NetworkGuid = networkGuid,
                MasterUser = uuid,
                MaxUsers = 2,
                TotalUsers = 1
            };

            httpContext.Response.AddHeader("GUID", networkGuid);

            dbContext.Networks.Add(network);
            await GetNetworkPassword.CreatePassword(httpContext.Response, dbContext, uuid, networkGuid);

            await dbContext.SaveChangesAsync();

            // Commit the transaction
            await transaction.CommitAsync();

            await Response.TextResponse(httpContext, "Network Created");
        }
        catch (Exception ex)
        {
            // Roll back the transaction if any error occurs
            await transaction.RollbackAsync();
            await Response.TextResponse(httpContext, $"Error: {ex.Message}", 500);
        }
    }
}