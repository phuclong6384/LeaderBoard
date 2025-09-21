using FluentValidation.TestHelper;
using Leaderboard.Application.Commands.SubmitScore;
using Leaderboard.Application.Queries.GetLeaderboard;

namespace Leaderboard.UnitTests.Application
{
    public class ValidatorsTests
    {
        [Fact]
        public void SubmitScoreCommandValidator_NegativeScore_Fails()
        {
            var validator = new SubmitScoreCommandValidator();
            var cmd = new SubmitScoreCommand(Guid.NewGuid(), -1);
            var res = validator.TestValidate(cmd);
            res.ShouldHaveValidationErrorFor(x => x.Score);
        }

        [Fact]
        public void GetLeaderboardQueryValidator_EmptyPlayerId_Fails()
        {
            var validator = new GetLeaderboardQueryValidator();
            var q = new GetLeaderboardQuery(Guid.Empty);
            var res = validator.TestValidate(q);
            res.ShouldHaveValidationErrorFor(x => x.PlayerId);
        }
    }
}
