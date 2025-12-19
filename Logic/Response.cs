using System.Net;
using System.Text;
class Response
{
    public static async Task TextResponse(HttpListenerResponse response, string text, int statusCode = 200)
    {
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

        foreach(var header in headers)
        {
            response.Headers.Add(header.Key, header.Value);
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
}