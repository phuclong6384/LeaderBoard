using System.Threading;
using System.Threading.Tasks;
using Leaderboard.Application.Commands.Reset;
using Leaderboard.Application.Interfaces;

namespace Leaderboard.Application.Commands
{
    public class ResetCommandHandler
    {
        private readonly IRankingRepository _ranking;

        public ResetCommandHandler(IRankingRepository ranking)
        {
            _ranking = ranking;
        }

        public async Task Handle(ResetCommand command, CancellationToken ct)
        {
            await _ranking.RemoveAllAsync(ct);
        }
    }
}
