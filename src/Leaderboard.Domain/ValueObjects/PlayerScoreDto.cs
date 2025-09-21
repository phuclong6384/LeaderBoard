namespace Leaderboard.Domain.ValueObjects
{
    public record PlayerScoreDto(Guid PlayerId, string PlayerName, int Score, int PlayerRank);
}
