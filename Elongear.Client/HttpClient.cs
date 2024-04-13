using NetCoreServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Elongear.Client
{
    public class HttpClient : NetCoreServer.HttpClientEx
    {
        public HttpClient(string address, int port) : base(address, port)
        {

        }
        protected override void OnReceivedResponse(HttpResponse response)
        {
            base.OnReceivedResponse(response);
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine(error.ToString());
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            base.OnReceived(buffer, offset, size);
        }
        
        protected override void OnReceivedResponseHeader(HttpResponse response)
        {
            Console.WriteLine(response.Header(0));
            base.OnReceivedResponseHeader(response);
        }
    }
}
