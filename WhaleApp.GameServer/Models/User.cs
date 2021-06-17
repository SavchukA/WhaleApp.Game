using System;

namespace WhaleApp.GameServer.Models
{
    public class User
    {
        public Guid UserId = Guid.NewGuid();
        
        public User(ClientConnection clientConnection)
        {
            ClientConnection = clientConnection;
        }
        
        public string Name { get; set; }

        public ClientConnection ClientConnection { get; set; }

        public void SendMessage(string message)
        {
            ClientConnection.Send(message);
        }
    }
}