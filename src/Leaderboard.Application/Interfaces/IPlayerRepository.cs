using Leaderboard.Domain.Entities;

namespace Leaderboard.Application.Interfaces
{
    public interface IPlayerRepository
    {
        Task<Player?> GetByIdAsync(Guid id, CancellationToken ct);
        Task UpsertAsync(Player player, CancellationToken ct);
    }
}
