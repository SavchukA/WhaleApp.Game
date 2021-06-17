using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WhaleApp.GameServer.Services;
using WhaleApp.GameServer.Settings;

namespace WhaleApp.GameServer.Models
{
    public static class Lobby
    {
        private static readonly List<GameController> Games = new List<GameController>();

        private static readonly Dictionary<Guid, GameController> PlayersInGames = new Dictionary<Guid, GameController>();

        private static GameSettings _gameSettings;

        public static void SetGameSettings(GameSettings settings)
        {
            _gameSettings = settings;
        }

        private static void StartGameForUser(User user, Guid? exceptGame = null)
        {
            var game = Games
                .OrderByDescending(x => x.FreePlacesCount())
                .FirstOrDefault(x => x.GameId != exceptGame && x.FreePlacesCount() > 0);

            if (game == null)
            {
                game = new GameController(
                    _gameSettings.MaxPlayersCount,
                    _gameSettings.PunchPower,
                    _gameSettings.CriticalPunchPower,
                    _gameSettings.CriticalPunchPercentage,
                    _gameSettings.MaxPunchesPerPeriod,
                    _gameSettings.PunchPeriodInSeconds,
                    _gameSettings.MaxPlayerHealth);

                Games.Add(game);
            }
            
            if (!game.TryAddNewPlayer(user))
            {
                Console.WriteLine($"Error with adding user: {user.Name}:{user.ClientConnection.SocketIp} to the game");
            }
            
            PlayersInGames.Add(user.UserId, game);
        }

        public static void HandleCommand(User actor, byte[] messageData, int length)
        {
            var data = Encoding.ASCII.GetString(messageData, 0, length);
            Console.WriteLine($"Received [{actor.ClientConnection.SocketIp}]: {data}");

            if (data.Contains("Name: "))
            {
                var name = data.Replace("Name: ", "");
                actor.Name = name;
                StartGameForUser(actor);
                return;
            }

            if (data.Contains("Punch ") || data.Contains("punch "))
            {
                var aimName = data.Replace("Punch ", "").Replace("punch ", "");
                if (PlayersInGames.ContainsKey(actor.UserId))
                {
                    PlayersInGames[actor.UserId].MakePunch(actor, aimName);
                }
            }

            if (data.Contains("Fight more"))
            {
                if (PlayersInGames.ContainsKey(actor.UserId) && PlayersInGames[actor.UserId].TryLeaveGame(actor))
                {
                    var exceptGameId = PlayersInGames[actor.UserId].GameId;
                    PlayersInGames.Remove(actor.UserId);
                    StartGameForUser(actor, exceptGameId);
                }
            }
        }
    }
}