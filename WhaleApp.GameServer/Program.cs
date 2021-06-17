using Microsoft.Extensions.Configuration;
using WhaleApp.GameServer.Infrastructure;
using WhaleApp.GameServer.Models;
using WhaleApp.GameServer.Settings;

namespace WhaleApp.GameServer
{
    class Program
    {
        static void Main()
        {
            var gameSettings = new ConfigurationBuilder()
                .AddJsonFile($"GameSettings.json", true, true)
                .Build()
                .Get<GameSettings>();
            
            Lobby.SetGameSettings(gameSettings ?? new GameSettings());
            
            TcpConnection.CreateTcpListener();
        }
    }
}