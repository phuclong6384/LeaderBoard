using Moq;
using Leaderboard.Application.Queries;
using Leaderboard.Application.Interfaces;
using Leaderboard.Domain.Entities;
using Leaderboard.Application.Config;
using Leaderboard.Application.Queries.GetLeaderboard;
using Leaderboard.Application.Builders;
using Leaderboard.Domain.Exceptions;

namespace Leaderboard.Application.Tests.QueryTests
{
    public class GetLeaderboardQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ExistingPlayer_ReturnsLeaderboard()
        {
            var playerRepo = new Mock<IPlayerRepository>();
            var rankingRepo = new Mock<IRankingRepository>();
            var config = new Mock<ILeaderboardConfig>();
            config.SetupGet(c => c.TopLimit).Returns(5);
            config.SetupGet(c => c.NearbyRange).Returns(1);

            var builder = new LeaderboardResultBuilder(rankingRepo.Object, playerRepo.Object, config.Object);

            var playerId = Guid.NewGuid();
            var playerName = "player_1";
            var nearbyRange = 1;
            var playerRank = 2;
            var score = 100;
            rankingRepo.Setup(r => r.GetScoreAsync(playerId, It.IsAny<CancellationToken>())).ReturnsAsync(score);
            rankingRepo.Setup(r => r.GetRankAsync(playerId, It.IsAny<CancellationToken>())).ReturnsAsync(playerRank);
            rankingRepo.Setup(r => r.GetTopAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(new List<(Guid, int)> { (playerId, score) });
            rankingRepo.Setup(r => r.GetNearByAsync(playerId, nearbyRange, It.IsAny<CancellationToken>())).ReturnsAsync(new List<(Guid, int)> { (playerId, score) });
            playerRepo.Setup(r => r.GetByIdAsync(playerId, It.IsAny<CancellationToken>())).ReturnsAsync(new Player(playerId, playerName));

            var handler = new GetLeaderboardQueryHandler(playerRepo.Object, builder);
            var res = await handler.Handle(new GetLeaderboardQuery(playerId), CancellationToken.None);

            Assert.Equal(playerRank, res.PlayerRank);
            Assert.Equal(score, res.PlayerScore);
            Assert.Single(res.TopPlayers);
        }

        [Fact]
        public async Task Handle_NonExistingPlayer_ThrowPlayerNotFoundException()
        {
            var playerRepo = new Mock<IPlayerRepository>();
            var rankingRepo = new Mock<IRankingRepository>();
            var config = new Mock<ILeaderboardConfig>();
            config.SetupGet(c => c.TopLimit).Returns(5);
            config.SetupGet(c => c.NearbyRange).Returns(1);

            var builder = new LeaderboardResultBuilder(rankingRepo.Object, playerRepo.Object, config.Object);

            var playerId = Guid.NewGuid();
            var nearbyRange = 1;
            rankingRepo.Setup(r => r.GetScoreAsync(playerId, It.IsAny<CancellationToken>())).ReturnsAsync((int?)null);
            rankingRepo.Setup(r => r.GetRankAsync(playerId, It.IsAny<CancellationToken>())).ReturnsAsync((int?)null);
            rankingRepo.Setup(r => r.GetTopAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(new List<(Guid, int)>());
            rankingRepo.Setup(r => r.GetNearByAsync(playerId, nearbyRange, It.IsAny<CancellationToken>())).ReturnsAsync(new List<(Guid, int)>());

            var handler = new GetLeaderboardQueryHandler(playerRepo.Object, builder);
            await Assert.ThrowsAsync<PlayerNotFoundException>(async () => await handler.Handle(new GetLeaderboardQuery(playerId), CancellationToken.None));
        }
    }
}
