namespace Leaderboard.Domain.Entities
{
    public class Player
    {
        public Guid PlayerId { get; private set; }
        public string Name { get; private set; }

        public Player(Guid playerId, string name)
        {
            PlayerId = playerId;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}
