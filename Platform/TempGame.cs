using ConnectionInterface;

namespace Platform
{
    public class TempGame : IGame
    {
        public string Name { get; private set; }
        public int Id { get; private set; }
        public string Description { get; private set; }
        public void LoadGame(byte[] gameState)
        {
            throw new System.NotImplementedException();
        }

        public byte[] SaveGame()
        {
            throw new System.NotImplementedException();
        }

        public TempGame()
        {
            Name = "MillGame";
            Id = 1;
            Description = "MillGame";
        }
    }
}
