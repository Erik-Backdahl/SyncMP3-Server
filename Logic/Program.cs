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
            HttpListenerContext HttpContext = await listener.GetContextAsync();
            SyncMp3Context dbContext = new SyncMp3Context(options);
            //Authenticate


            await LookForEndpoint(HttpContext, dbContext);
        }

    }

    private static async Task LookForEndpoint(HttpListenerContext httpContext, SyncMp3Context dbContext)
    {
        HttpListenerRequest request = httpContext.Request;
        string path = request.Url?.AbsolutePath ?? string.Empty;

        switch (path)
        {
            case "/ping":
                //await Endpoints.PingResponse(response);
                break;
            case "/create":
                await EasyEndpoints.CreateNewUser(httpContext, dbContext);
                break;
            default:
                //await Endpoints.DefaultResponse(response);
                break;
        }
    }


}