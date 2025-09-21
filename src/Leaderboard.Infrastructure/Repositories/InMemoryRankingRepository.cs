using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Leaderboard.Application.Interfaces;
using Leaderboard.Domain;
using Leaderboard.Domain.Entities;

namespace Leaderboard.Infrastructure.Repositories
{
    public class InMemoryRankingRepository : IRankingRepository
    {
        private readonly ConcurrentDictionary<Guid, PlayerEntry> _scores = new();
        private readonly object _resetLock = new();

        public Task AddScoreAsync(Guid playerId, int score, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            var playerEntry = new PlayerEntry()
            {
                Score = score,
                SubmittedAt = DateTime.UtcNow
            };

            _scores.AddOrUpdate(playerId, playerEntry, (pId, existing) => playerEntry);
            return Task.CompletedTask;
        }

        public Task<int?> GetScoreAsync(Guid playerId, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            if (_scores.TryGetValue(playerId, out var playerEntry)) 
                return Task.FromResult<int?>(playerEntry.Score);

            return Task.FromResult<int?>(null);
        }

        public Task RemoveAllAsync(CancellationToken ct)
        {            
            ct.ThrowIfCancellationRequested();

            lock (_resetLock)
            {
                _scores.Clear();
            }
            return Task.CompletedTask;
        }

        public Task<IEnumerable<(Guid PlayerId, int Score)>> GetTopAsync(int limit, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            var result = _scores
                .OrderByDescending(kv => kv.Value.Score)
                .ThenBy(entry => entry.Value.SubmittedAt)
                .Take(limit)
                .Select(kv => (kv.Key, kv.Value.Score));

            return Task.FromResult(result);
        }

        public Task<IEnumerable<(Guid PlayerId, int Score)>> GetNearByAsync(Guid playerId, int nearbyRange, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            var ordered = _scores
                .OrderByDescending(kv => kv.Value.Score)
                .ThenBy(entry => entry.Value.SubmittedAt)
                .Select(kv => (kv.Key, kv.Value.Score))
                .ToList();
            var index = ordered.FindIndex(x => x.Key == playerId);
            if (index < 0) 
                return Task.FromResult(Enumerable.Empty<(Guid, int)>());

            var start = Math.Max(0, index - nearbyRange);
            var end = Math.Min(ordered.Count - 1, index + nearbyRange);
            var result = ordered
                .Skip(start)
                .Take(end - start + 1)
                .Where(p => p.Key != playerId);
            
            return Task.FromResult(result);
        }

        public Task<int?> GetRankAsync(Guid playerId, CancellationToken ct)
        {
            if (!_scores.TryGetValue(playerId, out var playerScore)) 
                return Task.FromResult<int?>(null);

            var distinctHigherCount = _scores.Values
                .Select(s => s.Score)
                .Distinct()
                .Count(score => score > playerScore.Score);

            return Task.FromResult<int?>(distinctHigherCount + 1);
        }
    }
}
