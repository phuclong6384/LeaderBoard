using Leaderboard.Application.Interfaces;
using Leaderboard.Domain.Entities;

namespace Leaderboard.Infrastructure.Seed
{
    /// <summary>
    /// Seeds 20 players and their initial scores (including 4 players sharing the same score).
    /// The seeder is idempotent: if leaderboard already has entries it will not reseed.
    /// </summary>
    public class SeedData
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IRankingRepository _rankingRepository;
        private readonly ISubmissionRepository _submissionRepository;


        public SeedData(IPlayerRepository playerRepo, IRankingRepository rankingRepo, ISubmissionRepository submissionRepo)
        {
            _playerRepository = playerRepo;
            _rankingRepository = rankingRepo;
            _submissionRepository = submissionRepo;
        }


        public async Task SeedAsync(CancellationToken ct = default)
        {
            // if any data exists we consider the leaderboard seeded
            var existing = await _rankingRepository.GetTopAsync(1, ct);
            if (existing != null && existing.Any()) return;

            var players = new List<Player>
            {
                new Player(Guid.Parse("0a111111-1111-1111-1111-111111111111"), "Alice"),
                new Player(Guid.Parse("0a222222-2222-2222-2222-222222222222"), "Bob"),
                new Player(Guid.Parse("0a333333-3333-3333-3333-333333333333"), "Carol"),
                new Player(Guid.Parse("0a444444-4444-4444-4444-444444444444"), "Dave"),
                new Player(Guid.Parse("0a555555-5555-5555-5555-555555555555"), "Grace"),

                // 6 duplicates (score = 5000)
                new Player(Guid.Parse("0a666666-6666-6666-6666-666666666666"), "Frank"),
                new Player(Guid.Parse("0a777777-7777-7777-7777-777777777777"), "Eve"),
                new Player(Guid.Parse("0a888888-8888-8888-8888-888888888888"), "Heidi"),
                new Player(Guid.Parse("0a999999-9999-9999-9999-999999999999"), "Ivan"),
                new Player(Guid.Parse("0b111111-1111-1111-1111-111111111111"), "Judy"),
                new Player(Guid.Parse("0b222222-2222-2222-2222-222222222222"), "Karl"),

                // remaining players
                new Player(Guid.Parse("0b333333-3333-3333-3333-333333333333"), "Leo"),
                new Player(Guid.Parse("0b444444-4444-4444-4444-444444444444"), "Mallory"),
                new Player(Guid.Parse("0b555555-5555-5555-5555-555555555555"), "Neil"),
                new Player(Guid.Parse("0b666666-6666-6666-6666-666666666666"), "Olivia"),
                new Player(Guid.Parse("0b777777-7777-7777-7777-777777777777"), "Peggy"),
                new Player(Guid.Parse("0b888888-8888-8888-8888-888888888888"), "Quentin"),
                new Player(Guid.Parse("0b999999-9999-9999-9999-999999999999"), "Rupert"),
                new Player(Guid.Parse("0c111111-1111-1111-1111-111111111111"), "Sybil"),
                new Player(Guid.Parse("0c222222-2222-2222-2222-222222222222"), "Trent"),
            };

            var submission = new List<Submission>
            {
                new Submission(Guid.Parse("0a111111-1111-1111-1111-111111111111"), 10000),
                new Submission(Guid.Parse("0a222222-2222-2222-2222-222222222222"), 9000),
                new Submission(Guid.Parse("0a333333-3333-3333-3333-333333333333"), 9000),
                new Submission(Guid.Parse("0a444444-4444-4444-4444-444444444444"), 7000),
                new Submission(Guid.Parse("0a555555-5555-5555-5555-555555555555"), 6000),

                // 6 duplicates (score = 5000)
                new Submission(Guid.Parse("0a666666-6666-6666-6666-666666666666"), 5000),
                new Submission(Guid.Parse("0a777777-7777-7777-7777-777777777777"), 5000),
                new Submission(Guid.Parse("0a888888-8888-8888-8888-888888888888"), 5000),
                new Submission(Guid.Parse("0a999999-9999-9999-9999-999999999999"), 5000),
                new Submission(Guid.Parse("0b111111-1111-1111-1111-111111111111"), 5000),
                new Submission(Guid.Parse("0b222222-2222-2222-2222-222222222222"), 5000),

                // remaining players
                new Submission(Guid.Parse("0b333333-3333-3333-3333-333333333333"), 3500),
                new Submission(Guid.Parse("0b444444-4444-4444-4444-444444444444"), 3000),
                new Submission(Guid.Parse("0b555555-5555-5555-5555-555555555555"), 2500),
                new Submission(Guid.Parse("0b666666-6666-6666-6666-666666666666"), 2000),
                new Submission(Guid.Parse("0b777777-7777-7777-7777-777777777777"), 1500),
                new Submission(Guid.Parse("0b888888-8888-8888-8888-888888888888"), 1000),
                new Submission(Guid.Parse("0b999999-9999-9999-9999-999999999999"), 900),
                new Submission(Guid.Parse("0c111111-1111-1111-1111-111111111111"), 800),
                new Submission(Guid.Parse("0c222222-2222-2222-2222-222222222222"), 700),
            };

            foreach (var player in players)
            {
                await _playerRepository.UpsertAsync(player, ct);                
            }

            foreach (var sub in submission)
            {
                await _submissionRepository.AddAsync(sub, ct);
                await _rankingRepository.AddScoreAsync(sub.PlayerId, sub.Score, ct);
            }
        }
    }
}