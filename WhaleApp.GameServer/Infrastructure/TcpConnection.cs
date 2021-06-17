using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using WhaleApp.GameServer.Models;

namespace WhaleApp.GameServer.Infrastructure
{
    public class TcpConnection
    {
        public static void CreateTcpListener()
        {
            try
            {
                const int port = 13000;
                var localAddr = IPAddress.Parse("127.0.0.1");
                var server = new TcpListener(localAddr, port);
                server.Start();
                while (true)
                {
                    var client = server.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(ThreadProc, client);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine($"SocketException: {e}");
            }
            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }
        
        private static void ThreadProc(object obj)
        {
            var user = new User(new ClientConnection {Client = (TcpClient) obj});
            var socketIp = user.ClientConnection.SocketIp;
            while (user.ClientConnection.Client.Connected)
            {
                var bytes = new byte[256];
                Console.WriteLine("Connected -> " + socketIp);
                int i;
                while (user.ClientConnection.Client.Client != null && (i = user.ClientConnection.Client.Client.Receive(bytes)) != 0)
                {
                    Lobby.HandleCommand(user, bytes, i);
                }
                user.ClientConnection.Client.Close();
            }
            Console.WriteLine($"Disconnected -> {socketIp}");
        }
    }
}