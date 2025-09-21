using Leaderboard.Application.Interfaces;
using Leaderboard.Domain.Entities;
using Leaderboard.Application.DTOs;
using Leaderboard.Application.Config;
using Leaderboard.Application.Builders;
using Leaderboard.Domain.Exceptions;

namespace Leaderboard.Application.Commands.SubmitScore
{
    public class SubmitScoreCommandHandler
    {
        private readonly IPlayerRepository _players;
        private readonly ISubmissionRepository _submissions;
        private readonly IRankingRepository _ranking;
        private readonly ILeaderboardConfig _config;
        private readonly LeaderboardResultBuilder _builder;

        public SubmitScoreCommandHandler(
            IPlayerRepository players,
            ISubmissionRepository submissions,
            IRankingRepository ranking,
            ILeaderboardConfig config,
            LeaderboardResultBuilder builder)
        {
            _players = players;
            _submissions = submissions;
            _ranking = ranking;
            _config = config;
            _builder = builder;
        }

        public async Task<LeaderboardDto> Handle(SubmitScoreCommand command, CancellationToken ct)
        {
            var player = await _players.GetByIdAsync(command.PlayerId, ct);
            if (player == null)
            {
                throw new PlayerNotFoundException(command.PlayerId);
            }

            // persist submission history
            var submission = new Submission(command.PlayerId, command.Score);
            await _submissions.AddAsync(submission, ct);

            // update ranking - for this challenge accumulate: we assume new score is added to existing score
            int existingScore = await _ranking.GetScoreAsync(command.PlayerId, ct) ?? 0;
            var newScore = existingScore + command.Score;
            await _ranking.AddScoreAsync(command.PlayerId, newScore, ct);

            // build result
            var result = await _builder.BuildAsync(command.PlayerId, ct);
            return result;
        }
    }
}
