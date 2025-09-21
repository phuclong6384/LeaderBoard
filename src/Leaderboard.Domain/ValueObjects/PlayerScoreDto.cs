namespace Leaderboard.Domain.ValueObjects
{
    public record PlayerScoreDto(Guid PlayerId, string PlayerName, long Score, int PlayerRank);
}
