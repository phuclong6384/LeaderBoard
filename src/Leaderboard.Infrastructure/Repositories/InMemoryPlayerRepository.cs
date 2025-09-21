using System.Collections.Concurrent;
using Leaderboard.Application.Interfaces;
using Leaderboard.Domain.Entities;

namespace Leaderboard.Infrastructure.Repositories
{
    public class InMemoryPlayerRepository : IPlayerRepository
    {
        private readonly ConcurrentDictionary<Guid, Player> _store = new();

        public Task<Player?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            _store.TryGetValue(id, out var p);
            return Task.FromResult(p);
        }

        public Task UpsertAsync(Player player, CancellationToken ct)
        {
            _store.AddOrUpdate(player.PlayerId, player, (_, __) => player);
            return Task.CompletedTask;
        }
    }
}
