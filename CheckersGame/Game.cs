using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtificialIntelligence;
using GTInterfacesLibrary;
using GTInterfacesLibrary.GameEvents;
using GTInterfacesLibrary.MessageTypes;
using CheckersGame.Logic;

namespace CheckersGame
{
    public class Game : GTGameInterface
    {
        private GTPlatformManagerInterface PlatformGameManager;
        //private GTArtificialIntelligenceInterface<GTGameSpaceElementInterface, IPosition> AI;


        private GTGuiInterface GUI;
        private Logic.Logic logic;
        private Step step;

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
            if (logic.isGameOver())
            {
                //eventArgs.GameState = this.SaveGame();
                eventArgs.IsMyTurn = false;
                eventArgs.GamePhase = GamePhase.Ended;
                eventArgs.IsWon = true;
            }

            SendGameStateChangedEventArg(this, eventArgs);
        }

        public void RegisterGameManager(GTPlatformManagerInterface platformGameManager)
        {
            PlatformGameManager = platformGameManager;
            PlatformGameManager.SendGameStateChangedEvent += RecieveGameState;
        }

        public void RegisterArtificialIntelligence(IGTArtificialIntelligenceInterface artificialIntelligence)
        {
            //AI = (GTArtificialIntelligenceInterface<GTGameSpaceElementInterface, IPosition>)AiList.First(x => x.Name == artificialIntelligence.Name);
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
            if (step == null)
            {
                Position pos = new Position(7 - row, column);
                if (logic.getCurrentState().hasElementAt(pos))
                {
                    step = new Step(logic.getCurrentState().elementAt(pos), pos, null);
                }
            }
            else
            {
                Position pos = new Position(7 - row, column);
                step = new Step(step.element, step.from, pos);
                logic.updateGameSpace(step);
                step = null;
                stepWithNextUser();
            }

            gui.SetField(StateToBytes(logic.getCurrentState()));
        }

        public void RecieveGameState(object sender, GameStateChangedEventArgs gameStateChangedEventArgs)
        {
            if (gameStateChangedEventArgs.GamePhase == GamePhase.Started)
            {
                if (gameStateChangedEventArgs.GameType == GameType.Local)
                {
                    logic.addPlayer(new GTPlayer<Element, Position>().playerWithRealUser(1));
                    logic.addPlayer(new GTPlayer<Element, Position>().playerWithAI(null, 0));
                }
                else if (gameStateChangedEventArgs.GameType == GameType.Online)
                {
                    logic.addPlayer(new GTPlayer<Element, Position>().playerWithRealUser(1));
                    logic.addPlayer(new GTPlayer<Element, Position>().playerWithRealUser(2));
                }
            }
            else if (gameStateChangedEventArgs.GamePhase == GamePhase.Playing)
            {
                //LoadGame(gameStateChangedEventArgs.GameState);
                GameSpace currentState = (GameSpace)logic.getCurrentState();
                GUI.SetField(StateToBytes(currentState));
                //logic.nextPlayer;
                
                if (gameStateChangedEventArgs.GameType == GameType.Local)
                {
                    stepWithNextUser();
                }
            }
        }

        private void stepWithNextUser()
        {
            //logic.ChangePlayer();
            GameSpace state = (GameSpace)logic.getNextState();
            logic.state = state;
            logic.addPlayer(new GTPlayer<Element, Position>().playerWithRealUser(1));
            logic.addPlayer(new GTPlayer<Element, Position>().playerWithAI(null, 0));
            GUI.SetField(StateToBytes(state));
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
