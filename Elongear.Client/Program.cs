

namespace Elongear.Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // TCP server address
            string address = "127.0.0.1";
            if (args.Length > 0)
                address = args[0];

            // HTTP server port
            int port = 8080;
            if (args.Length > 1)
                port = int.Parse(args[1]);

            Console.WriteLine($"HTTP server address: {address}");
            Console.WriteLine($"HTTP server port: {port}");

            Console.WriteLine();

            string url = "http://" + address + ":8080";
            // Create a new HTTP client
            var client = new HttpClient(address, port);

            Console.WriteLine("Press Enter to stop the client or '!' to reconnect the client...");

            // Perform text input
            for (; ; )
            {
                var line = Console.ReadLine();
                if (string.IsNullOrEmpty(line))
                    break;
                var task = client.SendPostRequest(url, line);
                Console.WriteLine("Send post request");
                Console.WriteLine("Response is " + task.Result);
            }

            // Disconnect the client
            Console.Write("Client disconnecting...");
            client.Disconnect();
            Console.WriteLine("Done!");
        }
    }
}
