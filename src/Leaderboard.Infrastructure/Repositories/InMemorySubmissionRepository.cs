using System.Collections.Concurrent;
using Leaderboard.Application.Interfaces;
using Leaderboard.Domain.Entities;

namespace Leaderboard.Infrastructure.Repositories
{
    public class InMemorySubmissionRepository : ISubmissionRepository
    {
        private readonly ConcurrentBag<Submission> _submissions = new();

        public Task AddAsync(Submission submission, CancellationToken ct)
        {
            _submissions.Add(submission);
            return Task.CompletedTask;
        }
    }
}
