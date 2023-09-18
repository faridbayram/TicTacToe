
using System;
using WebSocketSharp.Server;

namespace wsServer
{
    class Program
    {
        public static void Main(string[] args)
        {
            WebSocketServer server = new WebSocketServer("ws://localhost:8585");
            server.AddWebSocketService<WebSocketSession>("/");

            server.Start();
            Console.ReadKey(true);
            server.Stop();
        }
    }
}