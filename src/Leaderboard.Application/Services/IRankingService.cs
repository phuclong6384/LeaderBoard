namespace Leaderboard.Application.Services
{
    public interface IRankingService
    {
        Task UpdateScoreAsync(Guid playerId, int newScore, CancellationToken ct);
        Task<int?> GetRankAsync(Guid playerId, CancellationToken ct);
    }
}
