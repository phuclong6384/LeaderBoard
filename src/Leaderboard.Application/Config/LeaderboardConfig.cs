namespace Leaderboard.Application.Config
{
    public class LeaderboardConfig : ILeaderboardConfig
    {
        public int TopLimit { get; set; } = 10;
        public int NearbyRange { get; set; } = 2;
        public int ResetIntervalHours { get; set; } = 24;
    }
}
