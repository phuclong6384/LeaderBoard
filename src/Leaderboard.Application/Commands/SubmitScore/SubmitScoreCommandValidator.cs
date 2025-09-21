using FluentValidation;

namespace Leaderboard.Application.Commands.SubmitScore
{
    public class SubmitScoreCommandValidator : AbstractValidator<SubmitScoreCommand>
    {
        public SubmitScoreCommandValidator()
        {
            RuleFor(x => x.PlayerId).NotEmpty();
            RuleFor(x => x.Score).GreaterThanOrEqualTo(0);
        }
    }
}
