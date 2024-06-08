using Elongear.Server.Database.Operators;
using Elongear.Server.Encryption;
using NetCoreServer;
using System;
using System.Reflection.PortableExecutable;


namespace Elongear.Server.Engine;

public class Session : NetCoreServer.HttpSession
{
    public Server MyServer => (Server)Server;

    private PostSubSession PostSubSession { get; set; }
    private GetSubSession GetSubSession { get; set; }
    public FileSubSession FileSubSession { get; set; }
    
    public Session(HttpServer server) : base(server) 
    {
        FileSubSession = BaseSubSession.Create<FileSubSession>(this);
        PostSubSession = BaseSubSession.Create<PostSubSession>(this);
        GetSubSession = BaseSubSession.Create<GetSubSession>(this);
    }
    public bool SendErrorResponseAsync(string message) =>
        SendResponseAsync(Response.MakeErrorResponse(message));

    public bool SendOkResponseAsync(int status = 200) =>
        SendResponseAsync(Response.MakeOkResponse(status));

    public bool SendSuccessResponseAsync(string message) =>
        SendResponseAsync(Response.MakeGetResponse(message));

    protected override void OnReceivedRequest(HttpRequest request)
    {
        switch (request.Method)
        {
            case "GET":
                {
                    var commands = RequestHeaders.UrlToCommands(request);
                    GetSubSession.InvokeCommand(commands[0], request);
                    return;
                }
            case "POST":
                {
                    var command = RequestHeaders.GetCommand(request);
                    PostSubSession.InvokeCommand(command, request);
                    return;
                }
            case "HEAD":
                {
                    SendResponseAsync(Response.MakeHeadResponse());
                    return;
                }
            default:
                {
                    return;
                }
        }
    }

    public bool SendImageAsync(byte[] image, string extension)
    {
        var response = Response.MakeGetResponse(image);
        response.SetContentType(extension);
        return SendResponseAsync(response);
    }
  
    public bool SendListenResponseAsync(string extension, int size, long start, long end)
    {
        var response = Response.MakeGetResponse();
        response
            .SetBegin(206)
            .SetContentType(extension)
            .SetHeader("Accept-Ranges", "bytes")
            .SetHeader("Connection", "Keep-Alive")
            .SetHeader("Content-Range", $"bytes {start}-{end}/{size}")
            .SetBodyLength((int)(end-start));

        var sent = SendResponseAsync(response);
        return sent;
    }

    public bool SendFileResponseAsync(string extension, long size)
    {
        var response = Response.MakeGetResponse();
        response
            .SetBegin(200)
            .SetContentType(extension)
            .SetHeader("fileSize", $"{size}")
            .SetBodyLength((int)size);
        var sent = SendResponseAsync(response);
        return sent;
    }

    protected override void OnReceived(byte[] buffer, long offset, long size)
    {
        if (FileSubSession.IsReceiving)
        {
            FileSubSession.Receive(buffer, size);
            return;
        }
        base.OnReceived(buffer, offset, size);
    }
    
    protected override void OnDisconnected()
    {
        base.OnDisconnected();
        FileSubSession.FileStream?.Close();
    }
}
