using System.Net;
using SyncMP3.Data;
using SyncMP3.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

class Program
{
    static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables()
            .Build();

        var options = new DbContextOptionsBuilder<SyncMp3Context>()
            .UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
            .Options;


        var listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:8080/");
        listener.Start();

        Console.WriteLine("Listening...");

        while (true)
        {
            HttpListenerContext httpContext = await listener.GetContextAsync();
            SyncMp3Context dbContext = new SyncMp3Context(options);

            if (await Authenticate.RequestValid(httpContext, dbContext))
            {
                await LookForEndpoint(httpContext, dbContext);
            }
            else
            {
                await Response.TextResponse(httpContext, "Invalid Request");
            }
        }
    }
    private static async Task LookForEndpoint(HttpListenerContext httpContext, SyncMp3Context dbContext)
    {
        try
        {


            HttpListenerRequest request = httpContext.Request;
            string path = request.Url?.AbsolutePath ?? string.Empty;

            switch (path)
            {
                case "/ping":
                    await EndpointEntry.Ping(httpContext, dbContext);////////
                    break;
                case "/get-music":
                    await EndpointEntry.GetMusic(httpContext, dbContext);////////
                    break;
                case "/upload":
                    await EndpointEntry.Upload(httpContext, dbContext);////////
                    break;
                case "/maintenance":
                    await EndpointEntry.Maintain(httpContext, dbContext);////////
                    break;
                case "/add-user":
                    await EndpointEntry.AddUser(httpContext, dbContext);
                    break;
                case "/create-network":
                    await EndpointEntry.CreateNewNetwork(httpContext, dbContext);
                    break;
                case "/generate-password":
                    await EndpointEntry.GeneratePassword(httpContext, dbContext);////////
                    break;
                case "/remove-user":
                    await EndpointEntry.RemoveUser(httpContext, dbContext);////////
                    break;
                case "/transfer-title":
                    await EndpointEntry.TransferTitle(httpContext, dbContext);////////
                    break;
                default:
                    await EndpointEntry.Default(httpContext, dbContext);
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await Response.TextResponse(httpContext, "Internal server error", 500);
            return;
        }
    }
}