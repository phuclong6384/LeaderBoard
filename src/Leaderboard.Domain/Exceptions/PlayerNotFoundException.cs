using System;

namespace Leaderboard.Domain.Exceptions
{
    public class PlayerNotFoundException : Exception
    {
        public PlayerNotFoundException(Guid playerId)
            : base($"Player with ID {playerId} was not found.")
        {
            PlayerId = playerId;
        }

        public Guid PlayerId { get; }
    }
}
