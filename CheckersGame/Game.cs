using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTInterfacesLibrary;
using GTInterfacesLibrary.GameEvents;
using GTInterfacesLibrary.MessageTypes;
using CheckersGame.Logic;

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
            //AI = (GTArtificialIntelligenceInterface<GTGameSpaceElementInterface, GTPosition>)artificialIntelligence;
        }

        byte[,] damaBackGround = { 
                { 11, 12, 11, 12, 11, 12, 11, 12 }, 
                { 12, 11, 12, 11, 12, 11, 12, 11 },
                { 11, 12, 11, 12, 11, 12, 11, 12 }, 
                { 12, 11, 12, 11, 12, 11, 12, 11 },
                { 11, 12, 11, 12, 11, 12, 11, 12 }, 
                { 12, 11, 12, 11, 12, 11, 12, 11 },
                { 11, 12, 11, 12, 11, 12, 11, 12 }, 
                { 12, 11, 12, 11, 12, 11, 12, 11 }};
        byte[,] damaField = { 
                { 2, 2, 2, 2, 2, 2, 2, 2 }, 
                { 2, 2, 2, 2, 2, 2, 2, 2 }, 
                { 2, 2, 2, 2, 2, 2, 2, 2 }, 
                { 2, 2, 2, 2, 2, 2, 2, 2 }, 
                { 2, 2, 2, 2, 2, 2, 2, 2 }, 
                { 2, 2, 2, 2, 2, 2, 2, 2 }, 
                { 2, 2, 2, 2, 2, 2, 2, 2 },
                { 2, 2, 2, 2, 2, 2, 2, 2 }};

        public void RegisterGui(GTGuiInterface gui)
        {
            GUI = gui;

            GUI.SetFieldBackground(damaBackGround);
            GUI.SetField(StateToBytes(logic.getCurrentState()));
            GUI.FieldClicked += GuiOnFieldClicked;
        }

        private void GuiOnFieldClicked(GTGuiInterface gui, int row, int column)
        {
            damaField[row, column] = (byte)((damaField[row, column] + 1) % 3);
            gui.SetField(damaField);
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

        public byte[,] StateToBytes(GTGameSpaceInterface<CheckersGame.Logic.Element, CheckersGame.Logic.Position> state)
        {
            byte[,] bytes = new byte[8, 8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    bytes[i, j] = 2;
                }
            }

            foreach (KeyValuePair<Position, Element> pair in state)
            {
                IPosition pos = pair.Key;
                GTGameSpaceElementInterface element = pair.Value;
                if (element.owner == 1)
                    bytes[7 - pos.coordinates()[0], pos.coordinates()[1]] = 0;
                else
                    bytes[7 - pos.coordinates()[0], pos.coordinates()[1]] = 1;
            }
            return bytes;
        }
    }
}
