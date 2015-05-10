using System;
using GTInterfacesLibrary;
using GTInterfacesLibrary.GameEvents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Platform.Model.Interface;

namespace PlatformUnitTests
{
    [TestClass]
    public partial class PlatformUnitTests
    {
        private IGameManager GameManager;
        private INetworkManager NetworkManager;

        private GTGameInterface GetFirstGame()
        {
            const int id = 1;
            const string name = "1Game";
            const string description = "1GameDescription";
            var testGame = new TestGame(id, name, description);
            return testGame;
        }

        private GTGameInterface GetSecondGame()
        {
            const int id = 2;
            const string name = "2Game";
            const string description = "2GameDescription";
            var testGame = new TestGame(id, name, description);
            return testGame;
        }

        private class TestGame : GTGameInterface
        {
            private GTPlatformManagerInterface PlatformGameManager;

            public TestGame(Int32 id, String name, String description)
            {
                Id = id;
                Name = name;
                Description = description;
                Data = new byte[0];
            }

            public event EventHandler<GameStateChangedEventArgs> SendGameStateChangedEventArg;
            public void SendGameState(GameStateChangedEventArgs currentGameStateChangedEventArgs)
            {
                throw new NotImplementedException();
            }

            public void RegisterGameManager(GTPlatformManagerInterface platformGameManager)
            {
                PlatformGameManager = platformGameManager;
            }

            public void RegisterArtificialIntelligence(IGTArtificialIntelligenceInterface artificialIntelligence)
            {
                //
            }

            public void RegisterGui(GTGuiInterface gui)
            {
                //
            }

            public void RecieveGameState(object sender, GameStateChangedEventArgs gameStateChangedEventArgs)
            {
                //
            }

            private byte[] Data;

            public string Name { get; private set; }
            public int Id { get; private set; }
            public string Description { get; private set; }
            public void LoadGame(byte[] gameState)
            {
                Data = gameState;
            }

            public byte[] SaveGame()
            {
                return Data;
            }
        }
    }
}
