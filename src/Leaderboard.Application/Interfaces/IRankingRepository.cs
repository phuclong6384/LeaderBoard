namespace Leaderboard.Application.Interfaces
{
    // Persistence for ranking - supports Redis Sorted Set or InMemory
    public interface IRankingRepository
    {
        Task AddScoreAsync(Guid playerId, int score, CancellationToken ct);
        Task<int?> GetScoreAsync(Guid playerId, CancellationToken ct);
        Task RemoveAllAsync(CancellationToken ct);
        Task<IEnumerable<(Guid PlayerId, int Score)>> GetTopAsync(int limit, CancellationToken ct);
        Task<IEnumerable<(Guid PlayerId, int Score)>> GetNearByAsync(Guid playerId, int nearbyRange, CancellationToken ct);
        Task<int?> GetRankAsync(Guid playerId, CancellationToken ct);
    }
}
