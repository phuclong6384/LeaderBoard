using Moq;
using Leaderboard.Application.Interfaces;
using Leaderboard.Application.Commands.Reset;
using Leaderboard.Application.Commands;

namespace Leaderboard.UnitTests.Application
{
    public class ResetCommandHandlerTests
    {
        [Fact]
        public async Task Handle_WhenCalled_RemovesAllEntries()
        {
            var repo = new Mock<IRankingRepository>();
            repo.Setup(r => r.RemoveAllAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask).Verifiable();
            var handler = new ResetCommandHandler(repo.Object);
            await handler.Handle(new ResetCommand(), CancellationToken.None);
            repo.Verify(r => r.RemoveAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
