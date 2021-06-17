using System;
using System.Collections.Generic;
using System.Linq;
using WhaleApp.GameServer.Models;

namespace WhaleApp.GameServer.Services
{
    public class GameController
    {
        public Guid GameId = Guid.NewGuid();

        private Dictionary<string, List<DateTime>> playerPunches = new Dictionary<string, List<DateTime>>();
        
        private List<Player> Players { get; set; } = new List<Player>();
        private readonly int _punchPower;
        private readonly int _criticalPunchPower;
        private readonly int _criticalPunchPercentage;
        private readonly int _maxPunchesPerPeriod;
        private readonly int _punchPeriodInSeconds;
        private readonly int _maxPlayerHealth;
        private readonly int _maxPlayersCount;

        public GameController(
            int maxPlayersCount,
            int punchPower,
            int criticalPunchPower,
            int criticalPunchPercentage,
            int maxPunchesPerPeriod,
            int punchPeriodInSeconds,
            int maxPlayerHealth)
        {
            _maxPlayersCount = maxPlayersCount;
            _punchPower = punchPower;
            _criticalPunchPower = criticalPunchPower;
            _criticalPunchPercentage = criticalPunchPercentage;
            _maxPunchesPerPeriod = maxPunchesPerPeriod;
            _punchPeriodInSeconds = punchPeriodInSeconds;
            _maxPlayerHealth = maxPlayerHealth;
        }

        public int FreePlacesCount()
            => _maxPlayersCount - Players.Count(x => x.Health > 0);

        public bool TryAddNewPlayer(User user)
        {
            if (Players.Count(x => x.Health > 0) >= _maxPlayersCount) return false;

            var newPlayer = new Player(user, _maxPlayerHealth);
            Players.ForEach(x => x.SendMessage($"New player in game: {newPlayer}"));
            Players.Add(newPlayer);
            var playerNames = string.Join("\n", Players.Select(x => x.ToString()));
            user.SendMessage($"Welcome to the game. Players in game: \n{playerNames}");
            return true;
        }

        public bool TryLeaveGame(User user)
        {
            var playerToLeave = Players.FirstOrDefault(x => x.User == user);

            if (playerToLeave == null) return true;

            if (playerToLeave.Health > 0) return false;
            
            Players.Remove(playerToLeave);

            return true;

        }

        public void MakePunch(User actor, string aimName)
        {
            var actorPlayer = Players.FirstOrDefault(x => x.User.Name == actor.Name);
            if (actorPlayer == null) return;
            
            if (actorPlayer.Health <= 0)
            {
                actorPlayer.SendMessage("Sorry, you are dead.");
                return;
            }

            if (playerPunches.ContainsKey(actor.Name))
            {
                if (playerPunches[actor.Name].Count(x => x > DateTime.Now.AddSeconds(-_punchPeriodInSeconds)) >=
                    _maxPunchesPerPeriod)
                {
                    actorPlayer.SendMessage("Too much punches. Keep calm!");
                    return;
                }
            }
            
            var aimPlayer = Players.FirstOrDefault(x => x.User.Name == aimName);

            if (aimPlayer == null)
            {
                actorPlayer.SendMessage($"Player with name: [{aimName}] was not found in game.");
                return;
            }

            if (aimPlayer.Health <= 0)
            {
                actorPlayer.SendMessage($"Player with name: [{aimName}] is already dead.");
                return;
            }

            var rand = new Random(DateTime.Now.Millisecond);
            if (rand.Next(100) < _criticalPunchPercentage)
            {
                aimPlayer.Health -= _criticalPunchPower;
            }
            else
            {
                aimPlayer.Health -= _punchPower;
            }

            if (playerPunches.ContainsKey(actor.Name))
            {
                playerPunches[actor.Name].Add(DateTime.Now);
            }
            else
            {
                playerPunches.Add(actor.Name, new List<DateTime> {DateTime.Now});
            }

            BroadcastMessage(aimPlayer.Health <= 0
                ? $"Player [{aimName}] is dead. Player [{actor.Name}] has killed him."
                : $"Player [{actor.Name}] has punched [{aimName}]. [{aimPlayer}]");
        }

        private void BroadcastMessage(string message)
        {
            Players.ForEach(x => x.User.SendMessage(message));
        }
    }
}