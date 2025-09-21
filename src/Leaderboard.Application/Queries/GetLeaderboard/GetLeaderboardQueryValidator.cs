using FluentValidation;

namespace Leaderboard.Application.Queries.GetLeaderboard
{
    public class GetLeaderboardQueryValidator : AbstractValidator<GetLeaderboardQuery>
    {
        public GetLeaderboardQueryValidator()
        {
            RuleFor(x => x.PlayerId).NotEmpty().WithMessage("PlayerId is required");
        }
    }
}
