using FluentValidation;
using Leaderboard.Application.Interfaces;
using Leaderboard.Infrastructure.Repositories;
using Leaderboard.Application.Commands;
using Leaderboard.Infrastructure.Jobs;
using Leaderboard.Application.Commands.SubmitScore;
using Leaderboard.Application.Config;
using Leaderboard.Application.Queries.GetLeaderboard;
using Leaderboard.Application.Queries;
using Leaderboard.Infrastructure.Seed;
using Leaderboard.Application.Builders;

var builder = WebApplication.CreateBuilder(args);

// Register all validators in the current assembly
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);


// bind LeaderboardConfig directly from configuration
builder.Services.Configure<LeaderboardConfig>(
builder.Configuration.GetSection("Leaderboard"));
builder.Services.AddSingleton<ILeaderboardConfig>(sp =>
{
    var config = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<LeaderboardConfig>>().Value;
    return config;
});

// repositories: InMemory by default. If REDIS_CONNECTION string provided, registers RedisRankingRepository
var redisConn = builder.Configuration.GetValue<string>("Redis:ConnectionString");
if (!string.IsNullOrWhiteSpace(redisConn))
{
    builder.Services.AddSingleton<IRankingRepository>(_ => new RedisRankingRepository(redisConn));
}
else
{
    builder.Services.AddSingleton<IRankingRepository, InMemoryRankingRepository>();
}

builder.Services.AddSingleton<IPlayerRepository, InMemoryPlayerRepository>();
builder.Services.AddSingleton<ISubmissionRepository, InMemorySubmissionRepository>();

// application services
builder.Services.AddSingleton<LeaderboardResultBuilder>();
builder.Services.AddSingleton<SubmitScoreCommandHandler>();
builder.Services.AddSingleton<GetLeaderboardQueryHandler>();
builder.Services.AddSingleton<ResetCommandHandler>();

// validators
builder.Services.AddSingleton<IValidator<SubmitScoreCommand>, SubmitScoreCommandValidator>();
builder.Services.AddSingleton<IValidator<GetLeaderboardQuery>, GetLeaderboardQueryValidator>();

// seed service
builder.Services.AddSingleton<SeedData>();

// background job
builder.Services.AddHostedService<ResetLeaderboardJob>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

if (!app.Environment.IsEnvironment("Testing"))
{
    // Seed data synchronously at startup
    using (var scope = app.Services.CreateScope())
    {
        var ranking = scope.ServiceProvider.GetRequiredService<IRankingRepository>();
        if (ranking is RedisRankingRepository)
        {
            var redisRankingRepository = ranking as RedisRankingRepository;
            await redisRankingRepository.RemoveAllAsync(CancellationToken.None);
        }

        var seeder = scope.ServiceProvider.GetRequiredService<SeedData>();
        await seeder.SeedAsync(CancellationToken.None);        
    }
}

app.Run();

public partial class Program { }