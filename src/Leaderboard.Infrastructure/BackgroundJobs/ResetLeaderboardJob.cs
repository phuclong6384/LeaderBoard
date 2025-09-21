using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Leaderboard.Application.Interfaces;
using Leaderboard.Application.Commands;
using Leaderboard.Application.Commands.Reset;
using Leaderboard.Application.Config;

namespace Leaderboard.Infrastructure.Jobs
{
    public class ResetLeaderboardJob : BackgroundService
    {
        private readonly ILogger<ResetLeaderboardJob> _logger;
        private readonly ILeaderboardConfig _config;
        private readonly ResetCommandHandler _resetHandler;

        public ResetLeaderboardJob(ILogger<ResetLeaderboardJob> logger, ILeaderboardConfig config, ResetCommandHandler resetHandler)
        {
            _logger = logger;
            _config = config;
            _resetHandler = resetHandler;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var delay = TimeSpan.FromHours(_config.ResetIntervalHours);
            _logger.LogInformation("ResetLeaderboardJob started. Interval: {Hours} hours", _config.ResetIntervalHours);
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(delay, stoppingToken);
                    _logger.LogInformation("Triggering leaderboard reset at {Time}", DateTimeOffset.UtcNow);
                    await _resetHandler.Handle(new ResetCommand(), stoppingToken);
                }
                catch (OperationCanceledException) { break; }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while resetting leaderboard");
                }
            }
        }
    }
}
