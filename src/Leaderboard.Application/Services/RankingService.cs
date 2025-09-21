using Leaderboard.Application.Interfaces;

namespace Leaderboard.Application.Services
{
    public class RankingService : IRankingService
    {
        private readonly IRankingRepository _repo;

        public RankingService(IRankingRepository repo)
        {
            _repo = repo;
        }

        public async Task UpdateScoreAsync(Guid playerId, int newScore, CancellationToken ct)
        {
            await _repo.AddScoreAsync(playerId, newScore, ct);
        }

        public async Task<int?> GetRankAsync(Guid playerId, CancellationToken ct)
        {
            return await _repo.GetRankAsync(playerId, ct);
        }
    }
}
