namespace Leaderboard.Domain.Entities
{
    public class Submission
    {
        public Guid SubmissionId { get; private set; }
        public Guid PlayerId { get; private set; }
        public int Score { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }

        public Submission(Guid playerId, int score)
        {
            SubmissionId = Guid.NewGuid();
            PlayerId = playerId;
            Score = score;
            CreatedAt = DateTimeOffset.UtcNow;
        }
    }
}
