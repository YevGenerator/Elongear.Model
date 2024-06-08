using NetCoreServer;


namespace Elongear.Server.Engine;

public class RequestHeaders
{
    public string LoginToken { get; set; } = "--";
    public string Command { get; set; } = "";
    public string PodcastId { get; set; } = "";
    public string UploadToken { get; set; } = "--";
    public string FileSize { get; set; } = "--";
    public string ContentType { get; set; } = "text/plain";
    public string Range { get; set; } = "";

    public void SetHeaders(HttpRequest request)
    {
        for (int i = 0; i < request.Headers; i++)
        {
            var header = request.Header(i);
            var key = header.Item1;
            var value = header.Item2;
            switch(key)
            {
                case "command":
                    {
                        Command = value;
                        break;
                    }
                case "auth_token":
                    {
                        LoginToken = value;
                        break;
                    }
                case "podcastId":
                    {
                        PodcastId = value;
                        break;
                    }
                case "upload_token":
                    {
                        UploadToken = value;
                        break;
                    }
                case "fileSize":
                    {
                        FileSize = value;
                        break;
                    }
                case "Content-Type":
                    {
                        ContentType = value;
                        break;
                    }
                case "Range":
                    {
                        Range = value;
                        break;
                    }
            }

        }
    }
    public static RequestHeaders Parse(HttpRequest request)
    {
        var headers = new RequestHeaders();
        headers.SetHeaders(request);
        return headers;
    }

    public static string[] UrlToCommands(HttpRequest request)
    {
        var commandUrl = 
            request.Url[0] == '/' 
            ? 
            request.Url.Remove(0, 1) : 
            request.Url;
        return commandUrl.Split(@"/");
    }

    public static string GetCommand(HttpRequest request)
    {
        for (int i = 0; i < request.Headers; i++)
        {
            var header = request.Header(i);
            var key = header.Item1;
            if (key == "command") return header.Item2;
        }
        return "";
    }
}
