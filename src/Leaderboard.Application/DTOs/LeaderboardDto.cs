using Leaderboard.Domain.ValueObjects;

namespace Leaderboard.Application.DTOs
{
    public record LeaderboardDto(int PlayerRank, int PlayerScore, IReadOnlyList<PlayerScoreDto> TopPlayers, IReadOnlyList<PlayerScoreDto> NearbyPlayers);
}
