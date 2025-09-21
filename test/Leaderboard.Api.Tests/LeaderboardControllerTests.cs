using System.Net;
using System.Net.Http.Json;
using Leaderboard.Application.DTOs;
using Leaderboard.Application.Interfaces;
using Leaderboard.Domain.Entities;
using Leaderboard.Infrastructure.Repositories;
using Leaderboard.Infrastructure.Seed;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Leaderboard.Api.Tests
{
    public class LeaderboardControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private Player _player_1 = new Player(Guid.NewGuid(), "Player_1");

        public LeaderboardControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder => { 
                builder.UseEnvironment("Testing");

                builder.ConfigureServices(services =>
                {
                    // Remove existing registration of IPlayerRepository
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(IPlayerRepository));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    // Register a fake or test repository
                    services.AddSingleton<IPlayerRepository, InMemoryPlayerRepository>();
                });
            });
            _client = _factory.CreateClient();            
        }

        [Fact]
        public async Task SubmitScore_ValidRequest_ReturnsExpectedResponse()
        {
            // Arrange
            var playerRepository = (InMemoryPlayerRepository)_factory.Services.GetService(typeof(IPlayerRepository));
            await playerRepository.UpsertAsync(_player_1, CancellationToken.None);

            var request = new { PlayerId = _player_1.PlayerId, Score = 100 };            

            // Act
            var response = await _client.PostAsJsonAsync("/api/submit", request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<LeaderboardDto>();
            Assert.NotNull(result);
            Assert.Equal(100, result!.PlayerScore);
            Assert.Equal(_player_1.PlayerId, result.TopPlayers[0].PlayerId);
        }

        [Fact]
        public async Task Leaderboard_GetExistingPlayer_ReturnsLeaderboardResult()
        {
            // Arrange
            var playerRepository = (InMemoryPlayerRepository)_factory.Services.GetService(typeof(IPlayerRepository));
            await playerRepository.UpsertAsync(_player_1, CancellationToken.None);
            var score = 50;
            await _client.PostAsJsonAsync("/api/submit", new { PlayerId = _player_1.PlayerId, Score = score });

            // Act
            var response = await _client.GetAsync($"/api/leaderboard?playerId={_player_1.PlayerId}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<LeaderboardDto>();
            Assert.NotNull(result);
            Assert.Equal(score, result!.PlayerScore);
        }

        [Fact]
        public async Task ResetLeaderboard_AfterSubmission_EmptiesScores()
        {
            // Arrange
            var playerId = Guid.NewGuid();
            await _client.PostAsJsonAsync("/api/submit", new { PlayerId = playerId, Score = 30 });

            // Act
            var resetResponse = await _client.PostAsync("/api/reset", null);

            // Assert
            Assert.Equal(HttpStatusCode.OK, resetResponse.StatusCode);

            var response = await _client.GetAsync($"/api/leaderboard?playerId={playerId}");
            var result = await response.Content.ReadFromJsonAsync<LeaderboardDto>();
            Assert.NotNull(result);
            Assert.Equal(0, result!.PlayerScore);
        }
    }
}
