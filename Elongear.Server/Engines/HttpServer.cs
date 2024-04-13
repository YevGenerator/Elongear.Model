using MySqlConnector;
using NetCoreServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Elongear.Server.Engines
{
    public class HttpServer : NetCoreServer.HttpServer
    {
        public HttpServer(IPAddress address, int port) : base(address, port)
        {
        }
        protected override NetCoreServer.TcpSession CreateSession() { return new HttpSession(this); }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"HTTP session caught an error: {error}");
        }
    }
    public class HttpSession : NetCoreServer.HttpSession
    {
        public HttpSession(NetCoreServer.HttpServer server) : base(server) { }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
            Console.WriteLine("Incoming: " + message);
            base.OnReceived(buffer, offset, size);
        }
        protected override async void OnReceivedRequest(HttpRequest request)
        {
            //var respon = Response.MakeGetResponse();
            if(request.Body == "get")
            {
                string message = "Server got your message " + request.Body;
                var myConnectionString = "server=127.0.0.1;uid=root;" +
                            "pwd=hackerdown123;database=elongear";
                using var connection = new MySqlConnection(myConnectionString);
                await connection.OpenAsync();
                using var command = new MySqlCommand("SELECT * FROM tag;", connection);
                using var reader = await command.ExecuteReaderAsync();
                object[] values = new object[2];
                var time = DateTime.Now;
                var response = Response.MakeGetResponse();
                response.SetContentType("application/json");
                StringBuilder stringBuilder = new('{');
                while (await reader.ReadAsync())
                {
                    stringBuilder.Append('[');
                    reader.GetValues(values);
                    stringBuilder.Append(string.Join(", ", values));
                    stringBuilder.Append("],");
                }
                stringBuilder.Remove(stringBuilder.Length-1, 1);
                stringBuilder.Append('}');
                SendResponseAsync(Response.MakeGetResponse(stringBuilder.ToString()));                
                SendResponseAsync(Response.MakeGetResponse("Надіслано із сервер все успішно"));
            }
            else
            {
                SendResponseAsync(Response.MakeGetResponse($"Повідомлення '{request.Body}' отримано"));
            }

        }

        protected override void OnReceivedRequestError(HttpRequest request, string error)
        {
            Console.WriteLine($"Request error: {error}");
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"HTTP session caught an error: {error}");
        }
    }
}
