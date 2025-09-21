using System.Threading;
using System.Threading.Tasks;
using Leaderboard.Application.Interfaces;
using Leaderboard.Application.DTOs;
using Leaderboard.Application.Queries.GetLeaderboard;
using Leaderboard.Application.Builders;
using Leaderboard.Domain.Exceptions;
using Leaderboard.Domain.Entities;

namespace Leaderboard.Application.Queries
{
    public class GetLeaderboardQueryHandler
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly LeaderboardResultBuilder _builder;

        public GetLeaderboardQueryHandler(IPlayerRepository playerRepository, LeaderboardResultBuilder builder)
        {
            _playerRepository = playerRepository;
            _builder = builder;
        }

        public async Task<LeaderboardDto> Handle(GetLeaderboardQuery query, CancellationToken ct)
        {
            var player = await _playerRepository.GetByIdAsync(query.PlayerId, ct);
            if (player == null)
            {
                throw new PlayerNotFoundException(query.PlayerId);
            }

            return await _builder.BuildAsync(query.PlayerId, ct);
        }
    }
}
