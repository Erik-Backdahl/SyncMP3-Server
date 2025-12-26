using System.Net;
using SyncMP3.Data;
using SyncMP3.Models;
class EndpointEntry
{
    internal static async Task Ping(HttpListenerContext httpContext, SyncMp3Context dbContext)
    {
        throw new NotImplementedException();
    }
    internal static async Task GetMusic(HttpListenerContext httpContext, SyncMp3Context dbContext)
    {
        throw new NotImplementedException();
    }
    internal static async Task Upload(HttpListenerContext httpContext, SyncMp3Context dbContext)
    {
        throw new NotImplementedException();
    }
    internal static async Task Maintain(HttpListenerContext httpContext, SyncMp3Context dbContext)
    {
        await GetNetworkPassword.Maintain();
    }
    internal static async Task AddUser(HttpListenerContext httpContext, SyncMp3Context dbContext)
    {
        await ModifyNetwork.AddUser(httpContext, dbContext);
    }
    internal static async Task CreateNewNetwork(HttpListenerContext httpContext, SyncMp3Context dbContext)
    {
        await CreateNetwork.CreateNewNetwork(httpContext, dbContext);
    }
    internal static async Task GeneratePassword(HttpListenerContext httpContext, SyncMp3Context dbContext)
    {
        throw new NotImplementedException();
    }
    internal static async Task RemoveUser(HttpListenerContext httpContext, SyncMp3Context dbContext)
    {
        throw new NotImplementedException();
    }
    internal static async Task TransferTitle(HttpListenerContext httpContext, SyncMp3Context dbContext)
    {
        throw new NotImplementedException();
    }
    internal static async Task Default(HttpListenerContext httpContext, SyncMp3Context dbContext)
    {
        await Response.TextResponse(httpContext, "Invalid Path", 400);
    }
}