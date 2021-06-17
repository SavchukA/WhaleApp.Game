using System.Net.Sockets;
using System.Text;

namespace WhaleApp.GameServer.Models
{
    public class ClientConnection
    {
        public TcpClient Client { get; set; }

        public string SocketIp => Client.Client?.RemoteEndPoint?.ToString();

        public void Send(string message)
        {
            Client.Client?.Send(Encoding.ASCII.GetBytes(message));
        }
    }
}