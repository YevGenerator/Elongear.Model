using NetCoreServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Elongear.Client
{
    internal class TcpClient : NetCoreServer.TcpClient
    {
        public TcpClient(string address, int port) : base(address, port) { }

        public void DisconnectAndStop()
        {
            _stop = true;
            DisconnectAsync();
            while (IsConnected)
                Thread.Yield();
        }

        protected override void OnConnected()
        {
            Console.WriteLine($"TCP client connected a new session with Id {Id}");
        }

        protected override void OnDisconnected()
        {
            Console.WriteLine($"TCP client disconnected a session with Id {Id}");

            // Wait for a while...
            Thread.Sleep(1000);

            // Try to connect again
            if (!_stop)
                ConnectAsync();
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
            Console.WriteLine(message);
            if (message == "File start")
            {
                var fileSizeBuffer = new byte[sizeof(long)];
                Receive(fileSizeBuffer);
                size = BitConverter.ToInt64(fileSizeBuffer);
                Console.WriteLine("File size is " + size);
                using var networkStream = new NetworkStream(Socket);
                using var fileStream = File.OpenWrite("C:\\Users\\yesman\\Desktop\\ToRemove\\Client\\v5.zip");
                long readSize = 0;
                var fileBuffer = new byte[OptionSendBufferSize];
                while(readSize<size)
                {
                    readSize += networkStream.Read(fileBuffer);
                    Console.WriteLine(readSize);
                    fileStream.Write(fileBuffer);
                }
                SendAsync("Received file");
                Console.WriteLine("Saved");
            }
            else
            {
                Console.WriteLine(message);
            }
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"TCP client caught an error with code {error}");
        }

        private bool _stop;
    }
}
