using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Threading;
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
        private GTGuiInterface GUI;
        private Logic.Logic logic;
        private string AIName;
        private Step step;
        private GameStateChangedEventArgs actualState;

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
            SendGameStateChangedEventArg(this, currentGameStateChangedEventArgs);
        }

        public void RegisterGameManager(GTPlatformManagerInterface platformGameManager)
        {
            PlatformGameManager = platformGameManager;
            PlatformGameManager.SendGameStateChangedEvent += RecieveGameState;
        }

        public void UnRegisterGameManager()
        {
            PlatformGameManager.SendGameStateChangedEvent -= RecieveGameState;
            PlatformGameManager = null;
        }

        public void RegisterArtificialIntelligence(String artificialIntelligenceName)
        {
            AIName = artificialIntelligenceName;
            logic.AIName = AIName;
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
            GUI.SetField(StateToField(logic.getCurrentState()));
            GUI.FieldClicked += GuiOnFieldClicked;
        }

        private void GuiOnFieldClicked(GTGuiInterface gui, int row, int column)
        {
            if (actualState == null)
                return;

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
                if (StepSupervisor.IsValidStep(logic.state, step))
                {
                    logic.updateGameSpace(step);
                    gui.SetField(StateToField(logic.getCurrentState()));
                    Dispatcher.CurrentDispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);
                    if (GameOver())
                        return;

                    StepSupervisor.RefreshState(logic.state);
                    logic.ChangePlayer();
                    stepWithNextUser();
                    if (GameOver())
                        return;
                }
                step = null;
            }   
        }

        private bool GameOver()
        {
            if (logic.isGameOver())
            {
                GameStateChangedEventArgs eventArgs = new GameStateChangedEventArgs();
                eventArgs.GamePhase = GamePhase.Ended;
                eventArgs.GameType = GameType.Local;
                if (logic.getWinner() == 1)
                    eventArgs.IsWon = true;
                else
                    eventArgs.IsWon = false;
                eventArgs.GameState = StateToBytes(logic.getCurrentState());
                SendGameState(eventArgs);

                logic = new Logic.Logic();
                logic.AIName = AIName;
                GUI.SetFieldBackground(damaBackGround);
                GUI.SetField(StateToField(logic.getCurrentState()));
                return true;
            }
            else
            {
                var eventArgs = new GameStateChangedEventArgs
                {
                    GamePhase = GamePhase.Playing,
                    IsWon = false,
                    GameState = StateToBytes(logic.getCurrentState())
                };
                SendGameState(eventArgs);
                return false;
            }
        }

        public void RecieveGameState(object sender, GameStateChangedEventArgs gameStateChangedEventArgs)
        {
            actualState = gameStateChangedEventArgs;

            if (actualState.GamePhase == GamePhase.Started)
            {
                IsCancelled = false;
                logic = new Logic.Logic();
                logic.AIName = AIName;
                GUI.SetFieldBackground(damaBackGround);
                GUI.SetField(StateToField(logic.getCurrentState()));

                if (actualState.GameType == GameType.Ai)
                {
                    logic.ChangePlayer();
                    logic.AIName = AIName;
                    int nextP = 1;
                    while (!GameOver() && !IsCancelled)
                    {                       
                        stepWithNextUser();
                        logic.state.nextPlayer = nextP;
                        nextP = 1 - nextP;
                        logic.ChangePlayer();
                        StepSupervisor.RefreshState(logic.state);
                        Dispatcher.CurrentDispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);
                        Thread.Sleep(1000);
                    }
                }
            }
            else if (actualState.GamePhase == GamePhase.Ended)
            {
                IsCancelled = true;
                actualState = null;
                logic = new Logic.Logic();
                logic.AIName = AIName;
                GUI.SetFieldBackground(damaBackGround);
                GUI.SetField(StateToField(logic.getCurrentState()));
            }
            else if (actualState.GamePhase == GamePhase.Playing)
            {

            }
        }

        private Boolean IsCancelled = false;

        private void stepWithNextUser()
        {
            GameSpace state = (GameSpace)logic.getNextState();
            logic.state = state;
            StepSupervisor.RefreshState(logic.state);
            GUI.SetField(StateToField(state));
        }

        public void LoadGame(byte[] gameState)
        {
            throw new NotImplementedException();
        }

        public byte[] SaveGame()
        {
            throw new NotImplementedException();
        }

        public byte[,] StateToField(GTGameSpaceInterface<CheckersGame.Logic.Element, CheckersGame.Logic.Position> state)
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

        public byte[] StateToBytes(GTGameSpaceInterface<CheckersGame.Logic.Element, CheckersGame.Logic.Position> state)
        {
            byte[] bytes = new byte[64];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    bytes[i*j] = 2;
                }
            }

            /*
            foreach (KeyValuePair<Position, Element> pair in state)
            {
                IPosition pos = pair.Key;
                GTGameSpaceElementInterface element = pair.Value;
                if (element.owner == 1)
                    bytes[7 - pos.coordinates()[0] * pos.coordinates()[1]] = 0;
                else
                    bytes[7 - pos.coordinates()[0] * pos.coordinates()[1]] = 1;
            }
            */
            return bytes;
        }
    }
}
