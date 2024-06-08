using Elongear.Server.Encryption;
using Elongear.Server.StringConstants;
using NetCoreServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elongear.Server.Engine;

public class FileSubSession: BaseSubSession
{
    private FilePathHelper FileHelper { get; set; } = new();
    public int BufferSize => 30 * Session.OptionSendBufferSize;
    public long CurrentReadPosition { get; set; } = 0;
    public FileStream FileStream { get; set; }
    public bool IsReceiving { get; set; } = false;
    public long ReadSize { get; set; } = 0;
    public long TotalSize { get; set; } = 0;

    public async void SendImage(HttpRequest request)
    {
        var commands = RequestHeaders.UrlToCommands(request);
        var imagePath = FileHelper.GetExistingImagePath(commands[1]);
        if (imagePath == "")
        {
            Session.SendErrorResponseAsync(ResponseMessages.NoImage);
            return;
        }
        var extension = FilePathHelper.GetExtension(imagePath);
        var bytes = await File.ReadAllBytesAsync(imagePath);
        Session.SendImageAsync(bytes, extension);
    }

    public async void UploadImage(HttpRequest request)
    {
        var headers = RequestHeaders.Parse(request);
        var values = TokenOperator.GetUserAndPodcastFromUploadToken(headers.LoginToken, headers.UploadToken);
        if (values.Length == 0)
        {
            Session.SendErrorResponseAsync(ResponseMessages.InvalidUploadToken);
            return;
        }
        FileHelper.CreatePodcastDirectory(values[1]);
        var imagePath = FileHelper.GetNewImagePath(values[1], headers.ContentType);
        await File.WriteAllBytesAsync(imagePath, request.BodyBytes);
        Session.SendSuccessResponseAsync(ResponseMessages.ImageAccepted);
    }
    public void OnListen(HttpRequest request)
    {
        var headers = RequestHeaders.Parse(request);
        var commands = RequestHeaders.UrlToCommands(request);
        var podcastId = commands[1];
        var podcastPath = FileHelper.GetExisitingPodcastPath(podcastId);
        var extension = FilePathHelper.GetExtension(podcastPath);
        using var fileStream = File.OpenRead(podcastPath);

        long start = 0;
        if (headers.Range == "")
        {
            start = 0;
        }
        else
        {
            var start_end = headers.Range.Split("=")[1].Split("-");
            start = long.Parse(start_end[0]);
        }
        
        Session.SendListenResponseAsync(extension, (int)fileStream.Length, start, (int)fileStream.Length);
        
        var buffer = new byte[BufferSize];
        fileStream.Seek(start, SeekOrigin.Begin);
        
        while (start < fileStream.Length)
        {
            var read = fileStream.Read(buffer, 0, BufferSize);
            Session.SendAsync(buffer);
            start += read;
        }
    }

    public void SendPodcast(HttpRequest request)
    {
        var commands = RequestHeaders.UrlToCommands(request);
        var podcastId = commands[1];
        var podcastPath = FileHelper.GetExisitingPodcastPath(podcastId);
        var extension = FilePathHelper.GetExtension(podcastPath);
        var len = new FileInfo(podcastPath).Length;
        using var fileStream = File.OpenRead(podcastPath);

        Session.SendFileResponseAsync(extension, len);

        long start = 0;
        var buffer = new byte[BufferSize];
        while (start < fileStream.Length)
        {
            start += fileStream.Read(buffer, 0, BufferSize);            
            Session.SendAsync(buffer);
        }
    }

    public void UploadPodcast(HttpRequest request)
    {
        var headers = RequestHeaders.Parse(request);
        var values = TokenOperator.GetUserAndPodcastFromUploadToken(headers.LoginToken, headers.UploadToken);
        if (values.Length == 0)
        {
            Session.SendErrorResponseAsync(ResponseMessages.InvalidUploadToken);
            return;
        }
        FileHelper.CreatePodcastDirectory(values[1]);
        var podcastPath = FileHelper.GetNewPodcastPath(values[1], headers.ContentType);
        ReadSize = 0;
        TotalSize = long.Parse(headers.FileSize);
        IsReceiving = true;
        FileStream = File.OpenWrite(podcastPath);
        Session.SendSuccessResponseAsync(ResponseMessages.ReadyToFileAccept);
    }

    public void Receive(byte[] buffer, long size)
    {
        FileStream.Write(buffer, 0, (int)size);
        ReadSize += size;
        if (ReadSize >= TotalSize)
        {
            IsReceiving = false;
            FileStream.Close();
            Session.SendSuccessResponseAsync("Прийняв");
        }
    }

    protected override void InitDict()
    {
       /* CommandMethods = new()
        {
            { Commands.Listen, OnListen },
            { Commands.UploadImage, UploadImage },
            { Commands.UploadPodcast, UploadPodcast},
            { Commands.Images, SendImage },
            { Commands.Download, SendPodcast }
        };*/
    }

}
