using NetCoreServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;

namespace Elongear.Server.Engines
{
    internal class TcpServer: NetCoreServer.TcpServer
    {
        public TcpServer(IPAddress address, int port) : base(address, port)
        {
        }
        protected override TcpSession CreateSession() { return new TcpSession(this); }
        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"TCP server caught an error with code {error}");
        }
    }

    public class TcpSession : NetCoreServer.TcpSession
    {
        public int Num { get; set; } = 0;
        public TcpSession(NetCoreServer.TcpServer server) : base(server)
        {
        }
        protected override void OnConnected()
        {
            Console.WriteLine("Connected with " + Id);
        }

        protected override async void OnReceived(byte[] buffer, long offset, long size)
        {
            string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
            Console.WriteLine("Incoming: " + message);

            // If the buffer starts with '!' the disconnect the current session
            if (message == "get")
            {
                var myConnectionString = "server=127.0.0.1;uid=root;" +
                        "pwd=hackerdown123;database=elongear";
                using var connection = new MySqlConnection(myConnectionString);
                await connection.OpenAsync();
                using var command = new MySqlCommand("SELECT * FROM tag;", connection);
                using var reader = await command.ExecuteReaderAsync();
                object[] values = new object[2];
                var time = DateTime.Now;
                while (await reader.ReadAsync())
                {
                    var value = reader.GetValues(values);
                    SendAsync(string.Join(" || ", values) + "\n");
                }
                SendAsync("Was sent all by " + (DateTime.Now - time).TotalSeconds);

            }
            else if (message == "insert")
            {
                Num++;
                var myConnectionString = "server=127.0.0.1;uid=root;" +
                       "pwd=hackerdown123;database=elongear";
                using var connection = new MySqlConnection(myConnectionString);
                await connection.OpenAsync();
                using var command = new MySqlCommand($"insert into tag (TagName) values (@val);", connection);
                command.Parameters.AddWithValue("val", "tag_" + Num);
                int i = await command.ExecuteNonQueryAsync();
                SendAsync("Inserted " + i + " rows");
            }
            else if (message == "More")
            {
                var myConnectionString = "server=127.0.0.1;uid=root;" +
                      "pwd=hackerdown123;database=elongear";
                using var connection = new MySqlConnection(myConnectionString);
                await connection.OpenAsync();

                var builder = new StringBuilder("");
                for (int k = 2000; k < 150000; k++)
                {
                    builder.Append("('tag_");
                    builder.Append(k);
                    builder.Append("'),");
                }
                builder.Remove(builder.Length - 1, 1);
                using var command = new MySqlCommand($"insert into tag (TagName) values " + builder.ToString() + ";", connection);
                Console.WriteLine(command.CommandText);
                int i = await command.ExecuteNonQueryAsync();
                SendAsync("Inserted " + i + " rows");
            }

            else if (message == "send")
            {
                var path = "C:\\Users\\yesman\\Desktop\\ToRemove\\Server\\v5.zip";

                using var fileStream = File.OpenRead(path);
                using var networkStream = new NetworkStream(Socket);

                SendAsync("File start");
                Send(BitConverter.GetBytes(fileStream.Length));
                long read = 0;
                var fileBuffer = new byte[OptionSendBufferSize];
                while (read < fileStream.Length)
                {
                    read += fileStream.Read(fileBuffer, 0, OptionSendBufferSize);
                    networkStream.Write(fileBuffer);
                }

                SendAsync("File end");
                Console.WriteLine("Sent");
            }
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"TCP session caught an error with code {error}");
        }
    }
}
