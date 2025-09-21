using Leaderboard.Application.Interfaces;
using StackExchange.Redis;

namespace Leaderboard.Infrastructure.Repositories
{
    // Uses Redis Sorted Set with score as the key. We store scores as double
    // Key: "leaderboard:zset"
    public class RedisRankingRepository : IRankingRepository, IDisposable
    {
        private readonly ConnectionMultiplexer _conn;
        private readonly IDatabase _db;
        private readonly string _key = "leaderboard:zset";
        private readonly string _keyIndex = "leaderboard:zset:player_index";

        public RedisRankingRepository(string configuration)
        {
            _conn = ConnectionMultiplexer.Connect(configuration);
            _db = _conn.GetDatabase();
        }

        public async Task AddScoreAsync(Guid playerId, int score, CancellationToken ct)
        {
            RedisValue memberValue = await _db.HashGetAsync(_keyIndex, playerId.ToString());
            if (memberValue.HasValue)
            {
                await _db.SortedSetRemoveAsync(_key, memberValue);
            }

            var submittedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            const long MaxTimestamp = 999_999_999_999_999;
            long reverseTimestamp = MaxTimestamp - submittedAt;
            var member = $"{score:D10}:{reverseTimestamp:D15}:{playerId}";

            await _db.SortedSetAddAsync(_key, member, score);
            await _db.HashSetAsync(_keyIndex, playerId.ToString(), member);
        }

        public async Task<int?> GetScoreAsync(Guid playerId, CancellationToken ct)
        {
            RedisValue memberValue = await _db.HashGetAsync(_keyIndex, playerId.ToString());
            if (!memberValue.HasValue) return 0;

            var score = await _db.SortedSetScoreAsync(_key, memberValue);
            if (!score.HasValue) return 0;

            return (int)score.Value;
        }

        public async Task RemoveAllAsync(CancellationToken ct)
        {
            await _db.KeyDeleteAsync(_key);
            await _db.KeyDeleteAsync(_keyIndex);
        }

        public async Task<IEnumerable<(Guid PlayerId, int Score)>> GetTopAsync(int limit, CancellationToken ct)
        {
            //// ZREVRANGE with scores
            var entries = await _db.SortedSetRangeByRankWithScoresAsync(_key, 0, limit - 1, Order.Descending);

            var results = new List<(Guid PlayerId, int Score)>();

            foreach (var entry in entries)
            {
                var parts = entry.Element.ToString().Split(':');
                if (parts.Length != 3)
                {
                    continue;
                }

                // Extract and parse the values from the parts.
                var originalScore = int.Parse(parts[0]);
                var playerId = Guid.Parse(parts[2]);

                results.Add((playerId, originalScore));
            }

            return results;
        }

        public async Task<IEnumerable<(Guid PlayerId, int Score)>> GetNearByAsync(Guid playerId, int nearbyRange, CancellationToken ct)
        {
            RedisValue memberValue = await _db.HashGetAsync(_keyIndex, playerId.ToString());
            if (!memberValue.HasValue)
                return Enumerable.Empty<(Guid, int)>();

            var rank = await _db.SortedSetRankAsync(_key, memberValue, Order.Descending);
            if (!rank.HasValue) 
                return Enumerable.Empty<(Guid, int)>();

            var start = Math.Max(0, (int)rank.Value - nearbyRange);
            var end = (int)rank.Value + nearbyRange;
            var entries = await _db.SortedSetRangeByRankWithScoresAsync(_key, start, end, Order.Descending);

            var result = entries
            .Where(e => e.Element.ToString().Contains(playerId.ToString()) == false)    
            .Select(e =>
            {
                var playerId = Guid.Parse(e.Element.ToString().Split(':')[2]);
                var score = (int)e.Score;
                return (playerId, score);
            })
            .ToList();

            return result;
        }

        public async Task<int?> GetRankAsync(Guid playerId, CancellationToken ct)
        {
            var score = await GetScoreAsync(playerId, ct);
            var all = await _db.SortedSetRangeByRankWithScoresAsync(_key, 0, -1, Order.Descending);
            var distinctScores = all.Select(e => (int)e.Score).Distinct().ToList();
            var index = distinctScores.FindIndex(s => s == (int)score.Value);
            if (index < 0) 
                return 0;
            return index + 1;
        }

        public void Dispose()
        {
            _conn?.Dispose();
        }
    }
}
