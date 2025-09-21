namespace Leaderboard.Application.Config
{
    public interface ILeaderboardConfig
    {
        int TopLimit { get; }
        int NearbyRange { get; }
        int ResetIntervalHours { get; }
    }
}
