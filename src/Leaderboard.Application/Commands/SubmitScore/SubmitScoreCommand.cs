namespace Leaderboard.Application.Commands.SubmitScore;

public record SubmitScoreCommand(Guid PlayerId, int Score);
