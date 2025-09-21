using Leaderboard.Domain.Entities;

namespace Leaderboard.UnitTests.Domain
{
    public class PlayerTests
    {
        [Fact]
        public void Player_Create_WithValidArguments_Succeeds()
        {
            // Arrange
            var id = Guid.NewGuid();
            var name = "Alice";

            // Act
            var player = new Player(id, name);

            // Assert
            Assert.Equal(id, player.PlayerId);
            Assert.Equal(name, player.Name);
        }

        [Fact]
        public void Player_Create_WithNullName_ThrowsArgumentNullException()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new Player(id, null!));
        }
    }
}
