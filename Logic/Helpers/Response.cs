using System.Net;
using System.Text;
class Response
{
    public static async Task TextResponse(HttpListenerContext httpContext, string text, int statusCode = 200)
    {
        HttpListenerResponse response = httpContext.Response;
        byte[] buffer = Encoding.UTF8.GetBytes(text);

        response.StatusCode = statusCode;
        response.ContentType = "text/plain";
        response.ContentLength64 = buffer.Length;

        await response.OutputStream.WriteAsync(buffer);
        response.Close();
    }

    public static async Task TextHeadersResponse(HttpListenerResponse response, string contentText, Dictionary<string, string> headers, int statusCode = 200)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(contentText);

        foreach (var header in headers)
        {
            response.AddHeader(header.Key, header.Value);
        }
        response.StatusCode = statusCode;
        response.ContentType = "text/plain";
        response.ContentLength64 = buffer.Length;

        await response.OutputStream.WriteAsync(buffer);
        response.Close();
    }

    public static async Task SongResponse(HttpListenerResponse response, byte[] songBytes, Dictionary<string, string> headers, int statusCode = 200)
    {

    }

    public static async Task IncrementalResponseHeader(HttpListenerResponse response, Dictionary<string, string> headers, bool finalize = false)
    {
        foreach (var header in headers)
        {
            response.AddHeader(header.Key, header.Value);
        }

        response.Close(); // Close the response after all chunks are written
    }
    public static async Task IncrementalResponseFinal(HttpListenerResponse response, string contentText, int statusCode = 200)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(contentText);

        response.StatusCode = statusCode;
        response.ContentType = "text/plain";
        response.ContentLength64 = buffer.Length;

        await response.OutputStream.WriteAsync(buffer);
        response.Close();
    }
}