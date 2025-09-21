using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Leaderboard.Application.Interfaces;
using Leaderboard.Application.DTOs;
using Leaderboard.Domain.ValueObjects;
using Leaderboard.Application.Config;

namespace Leaderboard.Application.Builders
{
    public class LeaderboardResultBuilder
    {
        private readonly IRankingRepository _ranking;
        private readonly IPlayerRepository _players;
        private readonly ILeaderboardConfig _config;

        public LeaderboardResultBuilder(IRankingRepository ranking, IPlayerRepository players, ILeaderboardConfig config)
        {
            _ranking = ranking;
            _players = players;
            _config = config;
        }

        public async Task<LeaderboardDto> BuildAsync(Guid playerId, CancellationToken ct)
        {
            var topPlayerScores = (await _ranking.GetTopAsync(_config.TopLimit, ct)).ToList();
            var nearbyPlayerScores = (await _ranking.GetNearByAsync(playerId, _config.NearbyRange, ct)).ToList();

            var playerScore = await _ranking.GetScoreAsync(playerId, ct) ?? 0;
            var playerRank = await _ranking.GetRankAsync(playerId, ct) ?? 0;

            var topPlayerScoreDtos = await GeneratePlayerScoreDtos(topPlayerScores, ct);
            var nearbyPlayerScoreDtos = await GeneratePlayerScoreDtos(nearbyPlayerScores, ct);

            return new LeaderboardDto(playerRank, playerScore, topPlayerScoreDtos, nearbyPlayerScoreDtos);
        }

        private async Task<List<PlayerScoreDto>> GeneratePlayerScoreDtos(List<(Guid PlayerId, int Score)> playerScores, CancellationToken ct)
        {
            var playerScoreDtos = new List<PlayerScoreDto>();
            foreach (var (pid, score) in playerScores)
            {
                var p = await _players.GetByIdAsync(pid, ct);
                var name = p?.Name ?? pid.ToString();
                var rank = await _ranking.GetRankAsync(pid, ct) ?? 0;
                playerScoreDtos.Add(new PlayerScoreDto(pid, name, score, rank));
            }

            return playerScoreDtos;
        }
    }
}
