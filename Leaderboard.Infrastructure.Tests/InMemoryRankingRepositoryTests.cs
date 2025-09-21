using Leaderboard.Infrastructure.Repositories;

namespace Leaderboard.Infrastructure.Tests
{
    public class InMemoryRankingRepositoryTests
    {
        [Fact]
        public async Task AddScoreAsync_MultipleConcurrentSubmissions_SumsCorrectly()
        {
            var repo = new InMemoryRankingRepository();
            var id = Guid.NewGuid();
            var tasks = new List<Task>();
            for (int i = 0; i < 100; i++)
            {
                tasks.Add(Task.Run(() => repo.AddScoreAsync(id, i, CancellationToken.None)));
            }
            await Task.WhenAll(tasks);
            var score = await repo.GetScoreAsync(id, CancellationToken.None);
            Assert.Equal(100, score);
        }

        [Fact]
        public async Task GetTopAsync_RankingTies_DenseRankingHandled()
        {
            var repo = new InMemoryRankingRepository();
            var a = Guid.NewGuid();
            var b = Guid.NewGuid();
            await repo.AddScoreAsync(a, 100, CancellationToken.None);
            await repo.AddScoreAsync(b, 100, CancellationToken.None);

            var top = (await repo.GetTopAsync(10, CancellationToken.None)).ToList();
            Assert.Equal(2, top.Count);
            Assert.Contains(top, t => t.PlayerId == a);
            Assert.Contains(top, t => t.PlayerId == b);

            var rankA = await repo.GetRankAsync(a, CancellationToken.None);
            var rankB = await repo.GetRankAsync(b, CancellationToken.None);
            Assert.Equal(rankA, rankB);
        }

        [Fact]
        public async Task RemoveAllAsync_WhileUpdating_NoRaceCondition()
        {
            var repo = new InMemoryRankingRepository();
            var id = Guid.NewGuid();
            var addTask = Task.Run(async () =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    await repo.AddScoreAsync(id, 1, CancellationToken.None);
                }
            });

            var resetTask = Task.Run(async () =>
            {
                // reset while add operations run
                await repo.RemoveAllAsync(CancellationToken.None);
            });

            await Task.WhenAll(addTask, resetTask);

            // Should not throw and repository should be in a consistent state (either null or >= 0)
            var final = await repo.GetScoreAsync(id, CancellationToken.None);
            Assert.True(final == null || final >= 0);
        }
    }
}
