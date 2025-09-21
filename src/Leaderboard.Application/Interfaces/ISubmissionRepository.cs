using Leaderboard.Domain.Entities;

namespace Leaderboard.Application.Interfaces
{
    public interface ISubmissionRepository
    {
        Task AddAsync(Submission submission, CancellationToken ct);
        // Optionally allow retrieval of history if needed
    }
}
