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

        private class TestGame : GTGameInterface
        {
            private GTPlatformManagerInterface PlatformGameManager;

            public TestGame(Int32 id, String name, String description)
            {
                Id = id;
                Name = name;
                Description = description;
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
                throw new NotImplementedException();
            }

            public void RegisterGui(GTGuiInterface gui)
            {
                throw new NotImplementedException();
            }

            public void RecieveGameState(object sender, GameStateChangedEventArgs gameStateChangedEventArgs)
            {
                throw new NotImplementedException();
            }

            public string Name { get; private set; }
            public int Id { get; private set; }
            public string Description { get; private set; }
            public void LoadGame(byte[] gameState)
            {
                throw new NotImplementedException();
            }

            public byte[] SaveGame()
            {
                throw new NotImplementedException();
            }
        }
    }
}
