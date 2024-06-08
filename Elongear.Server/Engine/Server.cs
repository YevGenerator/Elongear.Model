using Elongear.Server.Encryption;
using Elongear.Server.MailService;

using System.Net;
using System.Net.Sockets;

namespace Elongear.Server.Engine;

public class Server : NetCoreServer.HttpServer
{
    public TokenOperator TokenOperator { get; set; }
    public MailClient Mail { get; set; }
    private Dictionary<string, string> ConfigDictionary { get; set; }

    public Server(IPAddress address, int port) : base(address, port)
    {
        Mail = new() { RootPath ="http://192.168.0.103:8080/"};
        TokenOperator = new(Mail);
    }
    public string RootPath => ConfigDictionary["RootPath"];
    public string FileDirectoryName => ConfigDictionary["FileDirName"];
    public string FileDirectoryPath => Path.Combine(RootPath, FileDirectoryName);
    
    public async Task ReadConfigFromFile(string configPath)
    {
        using var reader = new StreamReader(configPath);
        ConfigDictionary.Add("RootPath", await reader.ReadLineAsync()??"");
        ConfigDictionary.Add("FileDirName", await reader.ReadLineAsync() ?? "Files");
        Mail.RootPath = RootPath;
        /*
        ConfigDictionary.Add("Port", await reader.ReadLineAsync() ?? "");
        ConfigDictionary.Add("RootPath", await reader.ReadLineAsync() ?? "");
        ConfigDictionary.Add("RootPath", await reader.ReadLineAsync() ?? "");
        */
    }
    protected override NetCoreServer.TcpSession CreateSession() { return new Session(this); }

    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"HTTP session caught an error: {error}");
        base.OnError(error);
    }
}
