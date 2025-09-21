using Moq;
using Leaderboard.Application.Interfaces;
using Leaderboard.Domain.Entities;
using Leaderboard.Application.Commands.SubmitScore;
using Leaderboard.Application.Config;
using Leaderboard.Application.Builders;

namespace Leaderboard.Application.Tests.CommandTest
{
    public class SubmitScoreCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ValidCommand_UpdatesScoreAndReturnsLeaderboard()
        {
            var playerRepo = new Mock<IPlayerRepository>();
            var submissionRepo = new Mock<ISubmissionRepository>();
            var rankingRepo = new Mock<IRankingRepository>();
            
            var score = 100;
            var nearbyRange = 2;
            var playerRank = 1;
            var config = new Mock<ILeaderboardConfig>();
            config.SetupGet(c => c.TopLimit).Returns(10);
            config.SetupGet(c => c.NearbyRange).Returns(nearbyRange);

            var builder = new LeaderboardResultBuilder(rankingRepo.Object, playerRepo.Object, config.Object);

            var playerId = Guid.NewGuid();
            playerRepo.Setup(r => r.GetByIdAsync(playerId, It.IsAny<CancellationToken>())).ReturnsAsync(new Player(playerId, "player_1"));
            playerRepo.Setup(r => r.UpsertAsync(It.IsAny<Player>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            submissionRepo.Setup(r => r.AddAsync(It.IsAny<Submission>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            rankingRepo.Setup(r => r.GetScoreAsync(playerId, It.IsAny<CancellationToken>())).ReturnsAsync(score);
            rankingRepo.Setup(r => r.AddScoreAsync(playerId, score, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // builder uses GetTop/GetRange/GetScore/GetRank - setup minimal
            rankingRepo.Setup(r => r.GetTopAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<(Guid, int)>());
            rankingRepo.Setup(r => r.GetNearByAsync(playerId, nearbyRange, It.IsAny<CancellationToken>())).ReturnsAsync(new List<(Guid, int)>());
            rankingRepo.Setup(r => r.GetRankAsync(playerId, It.IsAny<CancellationToken>())).ReturnsAsync(playerRank);

            var handler = new SubmitScoreCommandHandler(playerRepo.Object, submissionRepo.Object, rankingRepo.Object, config.Object, builder);

            var cmd = new SubmitScoreCommand(playerId, score);
            var res = await handler.Handle(cmd, CancellationToken.None);

            Assert.Equal(playerRank, res.PlayerRank);
            Assert.Equal(score, res.PlayerScore);
        }

        [Fact]
        public async Task Handle_MultipleSubmissions_AccumulatesScore()
        {
            var playerRepo = new Mock<IPlayerRepository>();
            var submissionRepo = new Mock<ISubmissionRepository>();
            var ranking = new Infrastructure.Repositories.InMemoryRankingRepository();
            var config = new Mock<ILeaderboardConfig>();
            config.SetupGet(c => c.TopLimit).Returns(10);
            config.SetupGet(c => c.NearbyRange).Returns(2);

            var builder = new LeaderboardResultBuilder(ranking, playerRepo.Object, config.Object);

            var playerId = Guid.NewGuid();
            var score_1 = 50;
            var score_2 = 70;
            playerRepo.Setup(r => r.GetByIdAsync(playerId, It.IsAny<CancellationToken>())).ReturnsAsync(new Player(playerId, "player_1"));
            playerRepo.Setup(r => r.UpsertAsync(It.IsAny<Player>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            submissionRepo.Setup(r => r.AddAsync(It.IsAny<Submission>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var handler = new SubmitScoreCommandHandler(playerRepo.Object, submissionRepo.Object, ranking, config.Object, builder);

            var cmd1 = new SubmitScoreCommand(playerId, score_1);
            var cmd2 = new SubmitScoreCommand(playerId, score_2);

            await handler.Handle(cmd1, CancellationToken.None);
            var res2 = await handler.Handle(cmd2, CancellationToken.None);

            Assert.Equal(score_1 + score_2, res2.PlayerScore);
        }
    }
}
