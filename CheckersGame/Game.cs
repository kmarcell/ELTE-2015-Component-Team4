using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTInterfacesLibrary;
using GTInterfacesLibrary.GameEvents;
using GTInterfacesLibrary.MessageTypes;

namespace CheckersGame
{
    public class Game : GTGameInterface
    {
        private GTPlatformManagerInterface PlatformGameManager;
        private GTArtificialIntelligenceInterface<GTGameSpaceElementInterface, GTPosition> AI;
        private GTGuiInterface GUI;
        private Logic.Logic logic;

		public Game()
        {
            Name = "CheckersGame";
            Id = 2;
            Description = "CheckersGame";
            logic = new Logic.Logic();
		}
        
        public string Name { get; private set; }
        public int Id { get; private set; }
        public string Description { get; private set; }

        public event EventHandler<GameStateChangedEventArgs> SendGameStateChangedEventArg;
        public void SendGameState(GameStateChangedEventArgs currentGameStateChangedEventArgs)
        {
            GameStateChangedEventArgs eventArgs = new GameStateChangedEventArgs();
            SendGameStateChangedEventArg(this, eventArgs);
        }

        public void RegisterGameManager(GTPlatformManagerInterface platformGameManager)
        {
            PlatformGameManager = platformGameManager;
            PlatformGameManager.SendGameStateChangedEvent += RecieveGameState;
        }

        public void RegisterArtificialIntelligence(IGTArtificialIntelligenceInterface artificialIntelligence)
        {
            AI = (GTArtificialIntelligenceInterface<GTGameSpaceElementInterface, GTPosition>)artificialIntelligence;
        }

        public void RegisterGui(GTGuiInterface gui)
        {
            GUI = gui;
            byte[,] damaBackGround = { 
                { 12, 11, 12, 11, 12, 11, 12, 11 }, 
                { 11, 12, 11, 12, 11, 12, 11, 12 },
                { 12, 11, 12, 11, 12, 11, 12, 11 }, 
                { 11, 12, 11, 12, 11, 12, 11, 12 },
                { 12, 11, 12, 11, 12, 11, 12, 11 }, 
                { 11, 12, 11, 12, 11, 12, 11, 12 },
                { 12, 11, 12, 11, 12, 11, 12, 11 }, 
                { 11, 12, 11, 12, 11, 12, 11, 12 }};
            byte[,] damaField = { 
                { 0, 0, 0, 0, 0, 0, 0, 0 }, 
                { 0, 0, 0, 0, 0, 0, 0, 0 }, 
                { 0, 0, 0, 0, 0, 0, 0, 0 }, 
                { 0, 0, 0, 0, 0, 0, 0, 0 }, 
                { 0, 0, 0, 0, 0, 0, 0, 0 }, 
                { 0, 0, 0, 0, 0, 0, 0, 0 }, 
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 }};

            GUI.SetFieldBackground(damaBackGround);
            GUI.SetField(damaField);
        }

        public void RecieveGameState(object sender, GameStateChangedEventArgs gameStateChangedEventArgs)
        {
            if (gameStateChangedEventArgs.GamePhase == GamePhase.Playing)
            {
            }
        }
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
