namespace WhaleApp.GameServer.Settings
{
    public class GameSettings
    {
        public int MaxPlayersCount { get; set; } = 2;

        public int PunchPower { get; set; } = 5;

        public int CriticalPunchPower { get; set; } = 20;

        public int CriticalPunchPercentage { get; set; } = 25;

        public int MaxPunchesPerPeriod { get; set; } = 5;

        public int PunchPeriodInSeconds { get; set; } = 15;

        public int MaxPlayerHealth { get; set; } = 100;
    }
}