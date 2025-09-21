using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using Leaderboard.Application.Commands;
using Leaderboard.Application.Queries;
using FluentValidation;
using FluentValidation.Results;
using Leaderboard.Application.DTOs;
using Leaderboard.Application.Commands.Reset;
using Leaderboard.Application.Commands.SubmitScore;
using Leaderboard.Application.Queries.GetLeaderboard;
using Leaderboard.Domain.Exceptions;

namespace Leaderboard.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class LeaderboardController : ControllerBase
    {
        private readonly SubmitScoreCommandHandler _submitHandler;
        private readonly GetLeaderboardQueryHandler _queryHandler;
        private readonly ResetCommandHandler _resetHandler;
        private readonly IValidator<SubmitScoreCommand> _submitValidator;
        private readonly IValidator<GetLeaderboardQuery> _queryValidator;

        public LeaderboardController(
            SubmitScoreCommandHandler submitHandler,
            GetLeaderboardQueryHandler queryHandler,
            ResetCommandHandler resetHandler,
            IValidator<SubmitScoreCommand> submitValidator,
            IValidator<GetLeaderboardQuery> queryValidator)
        {
            _submitHandler = submitHandler;
            _queryHandler = queryHandler;
            _resetHandler = resetHandler;
            _submitValidator = submitValidator;
            _queryValidator = queryValidator;
        }

        [HttpPost("submit")]
        public async Task<IActionResult> Submit([FromBody] SubmitScoreDto dto, CancellationToken ct)
        {
            var cmd = new SubmitScoreCommand(dto.PlayerId, dto.Score);
            var validation = await _submitValidator.ValidateAsync(cmd, ct);
            if (!validation.IsValid) return BadRequest(validation.Errors);

            try
            {
                var result = await _submitHandler.Handle(cmd, ct);
                return Ok(result);
            }
            catch (PlayerNotFoundException ex)
            {
                return NotFound(new ValidationFailure("PlayerId", ex.Message));
            }
        }

        [HttpGet("leaderboard")]
        public async Task<IActionResult> Leaderboard([FromQuery] Guid playerId, CancellationToken ct)
        {
            var query = new GetLeaderboardQuery(playerId);
            var validation = await _queryValidator.ValidateAsync(query, ct);
            if (!validation.IsValid) return BadRequest(validation.Errors);

            try
            {
                var result = await _queryHandler.Handle(query, ct);
                return Ok(result);
            }
            catch (PlayerNotFoundException ex)
            {
                return NotFound(new ValidationFailure("PlayerId", ex.Message));
            }
        }

        [HttpPost("reset")]
        public async Task<IActionResult> Reset(CancellationToken ct)
        {
            await _resetHandler.Handle(new ResetCommand(), ct);
            return Ok();
        }

        public record SubmitScoreDto(Guid PlayerId, int Score);
    }
}
