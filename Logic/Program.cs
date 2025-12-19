using System.Net;
using SyncMP3.Data;
using SyncMP3.Models;
class Program
{
    static async Task Main(string[] args)
    {
        var listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:8080/");
        listener.Start();

        Console.WriteLine("Listening...");

        while (true)
        {
            HttpListenerContext context = await listener.GetContextAsync();
            await LookForEndpoint(context.Request, context.Response);
        }
        
    }

    private static async Task LookForEndpoint(HttpListenerRequest request, HttpListenerResponse response)
    {
        string path = request.Url?.AbsolutePath ?? string.Empty;

        switch (path)
        {
            case "/ping":
                await Endpoints.PingResponse(response);
                break;

            case "/menu":
                await Endpoints.MenuResponse(response);
                break;

            case "/create":
                await Endpoints.CreateResponse(response);
                break;

            default:
                await Endpoints.DefaultResponse(response);
                break;
        }
    }

    
}
