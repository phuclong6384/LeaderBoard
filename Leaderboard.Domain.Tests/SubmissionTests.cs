using Leaderboard.Domain.Entities;

namespace Leaderboard.UnitTests.Domain
{
    public class SubmissionTests
    {
        [Fact]
        public void Submission_Create_WithValidArguments_Succeeds()
        {
            var playerId = Guid.NewGuid();
            int score = 1200;

            var s = new Submission(playerId, score);

            Assert.Equal(playerId, s.PlayerId);
            Assert.Equal(score, s.Score);
            Assert.True((DateTimeOffset.UtcNow - s.CreatedAt).TotalSeconds < 5);
        }
    }
}
