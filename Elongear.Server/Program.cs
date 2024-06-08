using System.Net;


namespace Elongear.Server;

internal class Program
{
    static void Main(string[] args)
    {
        int port = 8080;
        if (args.Length > 0)
            port = int.Parse(args[0]);
        

        Console.WriteLine($"HTTP server port: {port}");
        Console.WriteLine($"HTTP server website: http://localhost:{port}");

        Console.WriteLine();

        // Create a new HTTP server
        var server = new Engine.Server(IPAddress.Any, port);

        // Start the server
        Console.Write("Server starting...");
        server.Start();
        Console.WriteLine("Done!");

        Console.WriteLine("Press Enter to stop the server or '!' to restart the server...");

        // Perform text input
        for (; ; )
        {
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
                break;
            if(line=="!")
            {
                server.Restart();
            }

            // Restart the server
        }

        // Stop the server
        Console.Write("Server stopping...");
        server.Stop();
        Console.WriteLine("Done!");
    }
}
