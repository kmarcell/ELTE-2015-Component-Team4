using System;
using System.Collections.Generic;
using System.IO;
using GTInterfacesLibrary;
using GTInterfacesLibrary.GameEvents;
using System.Windows.Threading;
using System.Threading;

namespace GTMillGameLogic
{
    public class GTGame : GTGameInterface
    {
        private GTPlatformManagerInterface _PlatformGameManager;
        private GTGuiInterface _Gui;
        private readonly GTMillGameLogic _Logic;
        private RandomAI _RandomAI = new RandomAI();
        private CorrectAi _CorrectAI = new CorrectAi();
        private AlphaBetaAi _AlphaBetaAI = new AlphaBetaAi();
        private GTMillPosition selectedPosition;
        private string AIName;

        public GTGame()
        {
            Name = "MillGame";
            Id = 1;
            Description = "MillGame";
            _Logic = new GTMillGameLogic();
        }

        public string Name { get; private set; }
        public int Id { get; private set; }
        public string Description { get; private set; }

        public event EventHandler<GameStateChangedEventArgs> SendGameStateChangedEventArg;
        public void SendGameState(GameStateChangedEventArgs currentGameStateChangedEventArgs)
        {
            GameStateChangedEventArgs eventArgs = new GameStateChangedEventArgs();
            if (_Logic.isGameOver()) {

                eventArgs.GameState = this.SaveGame();
                eventArgs.IsMyTurn = false;
                eventArgs.GamePhase = GamePhase.Ended;
                eventArgs.IsWon = true;
            }

            SendGameStateChangedEventArg(this, eventArgs);
        }

        public void RegisterGameManager(GTPlatformManagerInterface platformGameManager)
        {
            _PlatformGameManager = platformGameManager;
            _PlatformGameManager.SendGameStateChangedEvent += RecieveGameState;
        }
        
        public void UnRegisterGameManager()
        {
            _PlatformGameManager.SendGameStateChangedEvent -= RecieveGameState;
            _PlatformGameManager = null;
        }

        public void RegisterArtificialIntelligence(String artificialIntelligenceName)
        {
            AIName = artificialIntelligenceName;
            _AlphaBetaAI._Logic = this._Logic;
            _CorrectAI._Logic = this._Logic;
            _RandomAI._Logic = this._Logic;
        }

        byte[,] malomBackGround = { 
                { 0, 9, 9, 4, 9, 9, 1 }, 
                { 10, 0, 9, 8, 9, 1, 10 },
                { 10, 10, 0, 6, 1, 10, 10 }, 
                { 7, 8, 5, 11, 7, 8, 5 },
                { 10, 10, 3, 4, 2, 10, 10 }, 
                { 10, 3, 9, 8, 9, 2, 10 },
                { 3, 9, 9, 6, 9, 9, 2 }};

        byte[,] malomField
        {
            get
            {
                return new byte[,] {{ 2, 2, 2, 2, 2, 2, 2 }, 
                                    { 2, 2, 2, 2, 2, 2, 2 }, 
                                    { 2, 2, 2, 2, 2, 2, 2 },
                                    { 2, 2, 2, 2, 2, 2, 2 },
                                    { 2, 2, 2, 2, 2, 2, 2 },
                                    { 2, 2, 2, 2, 2, 2, 2 },
                                    { 2, 2, 2, 2, 2, 2, 2 }};
            }
        }

        public void RegisterGui(GTGuiInterface gui)
        {
            _Gui = gui;
            _Gui.SetFieldBackground(malomBackGround);
            _Gui.SetField(malomField);
            _Gui.FieldClicked += GuiOnFieldClicked;
        }

        private void GuiOnFieldClicked(GTGuiInterface gui, int row, int column)
        {
            if (malomBackGround[row, column] == 9
                || malomBackGround[row, column] == 10
                || malomBackGround[row, column] == 11)
            {
                return;
            }

            GTMillPosition p = convertFromUI(row, column);
            GTMillGameSpace currentState = (GTMillGameSpace)_Logic.getCurrentState();
            GTMillGameStep step = new GTMillGameStep(new GTMillGameElement(1, 1, _Logic.nextPlayer.id), GTMillPosition.Nowhere(), GTMillPosition.Nowhere());

            bool goodStep = false;
            int gamePhase = _Logic.gamePhase;
            if (_Logic.gamePhase == 0)
            {
                if (!currentState.hasElementAt(p))
                {
                    goodStep = true;
                    step = new GTMillGameStep(new GTMillGameElement(1, 1, _Logic.nextPlayer.id), GTMillPosition.Nowhere(), p);
                }
            } else if (selectedPosition == null) {
                // selection
                if (currentState.hasElementAt(p))
                {
                    GTMillGameElement e = _Logic.getCurrentState().elementAt(p);
                    step = new GTMillGameStep(e, selectedPosition, p);
                    if (e.owner == _Logic.nextPlayer.id)
                    {
                        selectedPosition = p;
                    }
                }
                
            }
            else if (selectedPosition.Equals(GTMillPosition.Nowhere()))
            {
                // remove after Mill
                GTMillGameElement e = _Logic.getCurrentState().elementAt(p);
                step = new GTMillGameStep(e, selectedPosition, p);
                if (e.owner != _Logic.nextPlayer.id)
                {
                    selectedPosition = null;
                    goodStep = true;
                }
            }
            else
            {
                // step
                GTMillGameElement e = _Logic.getCurrentState().elementAt(p);
                step = new GTMillGameStep(e, selectedPosition, p);
                GTMillGameSpace newState = (GTMillGameSpace)currentState.stateWithStep(step);
                selectedPosition = null;

                List<GTGameSpaceInterface<GTMillGameElement, GTMillPosition>> availableStates = ((GTMillGameStateGenerator)_Logic.getStateGenerator()).availableStatesFrom(currentState, _Logic.nextPlayer, false).Result;
                foreach (GTMillGameSpace state in availableStates)
                {
                    if (newState.Equals(state))
                    {
                        goodStep = true;
                    }
                }

                if (goodStep && GTMillGameMillDetector.detectMillOnPositionWithStateForUser(p, currentState, _Logic.nextPlayer.id)) {
                    selectedPosition = GTMillPosition.Nowhere();
                }
            }

            if (!goodStep)
            {
                return;
            }

            _Logic.updateGameSpace(step);
            currentState = (GTMillGameSpace)_Logic.getCurrentState();
            _Gui.SetField(convertStateToField(currentState));
            Dispatcher.CurrentDispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);
            Thread.Sleep(1000);
            _Logic.playerDidStep();

            SendGameState(null);

            stepWithNextUser();
        }

        private void stepWithNextUser()
        {
            if (!_Logic.nextPlayer.isAI)
            {
                return;
            }

            GTMillGameSpace state = (GTMillGameSpace)getNextState();
            Thread.Sleep(100);
            _Logic.SetState(state);
            _Gui.SetField(convertStateToField(state));
            Dispatcher.CurrentDispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);
            _Logic.playerDidStep();
            SendGameState(null);
            stepWithNextUser();
        }

        private GTMillPosition convertFromUI(int row, int column)
        {
            int x = column / 3;
            int y = row / 3;

            int z = 0;
            if (row == 1 || row == 5 || column == 1 || column == 5)
            {
                z = 1;
            }
            else if (row == 2 || row == 4 || column == 2 || column == 4)
            {
                z = 2;
            }

            return new GTMillPosition(x, y, z);
        }

        public void RecieveGameState(object sender, GameStateChangedEventArgs gameStateChangedEventArgs)
        {
            if (gameStateChangedEventArgs.GamePhase == GamePhase.Playing)
            {
                LoadGame(gameStateChangedEventArgs.GameState);
                GTMillGameSpace currentState = (GTMillGameSpace)_Logic.getCurrentState();
                _Gui.SetField(convertStateToField(currentState));
                _Logic.playerDidStep();
                stepWithNextUser();
            }
            if (gameStateChangedEventArgs.GamePhase == GamePhase.Started)
            {
                GTPlayerInterface<GTMillGameElement, GTMillPosition> p;
                if (gameStateChangedEventArgs.GameType == GameType.Local)
                {
                    p = new GTPlayer<GTMillGameElement, GTMillPosition>().playerWithRealUser(1);
                    p.figuresInitial = 9;
                    p.figuresRemaining = 9;
                    _Logic.addPlayer(p);

                    p = new GTPlayer<GTMillGameElement, GTMillPosition>().playerWithAI(null, 2);
                    _Logic.addPlayer(p);
                    p.figuresInitial = 9;
                    p.figuresRemaining = 9;
                }
                else if (gameStateChangedEventArgs.GameType == GameType.Ai)
                {
                    p = new GTPlayer<GTMillGameElement, GTMillPosition>().playerWithAI(null, 1);
                    _Logic.addPlayer(p);
                    p.figuresInitial = 9;
                    p.figuresRemaining = 9;

                    p = new GTPlayer<GTMillGameElement, GTMillPosition>().playerWithAI(null, 2);
                    _Logic.addPlayer(p);
                    p.figuresInitial = 9;
                    p.figuresRemaining = 9;
                }
                else if (gameStateChangedEventArgs.GameType == GameType.Online)
                {
                    p = new GTPlayer<GTMillGameElement, GTMillPosition>().playerWithRealUser(1);
                    p.figuresInitial = 9;
                    p.figuresRemaining = 9;
                    _Logic.addPlayer(p);

                    p = new GTPlayer<GTMillGameElement, GTMillPosition>().playerWithRealUser(2);
                    p.figuresInitial = 9;
                    p.figuresRemaining = 9;
                    _Logic.addPlayer(p);
                }

                stepWithNextUser();
            }
        }

        public void LoadGame(byte[] gameState)
        {
            GTMillGameSpace newState = new GTMillGameSpace();
            MemoryStream memoryStream = new MemoryStream(gameState);
            BinaryReader reader = new BinaryReader(memoryStream);
            long pos = 0;
            while (pos < reader.BaseStream.Length)
            {
                int x = reader.ReadInt32();
                int y = reader.ReadInt32();
                int z = reader.ReadInt32();

                int id = reader.ReadInt32();
                int type = reader.ReadInt32();
                int owner = reader.ReadInt32();

                newState.setElementAt(new GTMillPosition(x, y, z), new GTMillGameElement(id, type, owner));

                pos += sizeof(int) * 6;
            }

            _Logic.SetState(newState);
        }

        public byte[] SaveGame()
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(memoryStream);
            foreach (KeyValuePair<GTMillPosition, GTMillGameElement> kv in _Logic.getCurrentState())
            {
                writer.Write(kv.Key.x);
                writer.Write(kv.Key.y);
                writer.Write(kv.Key.z);
                writer.Write(kv.Value.id);
                writer.Write(kv.Value.type);
                writer.Write(kv.Value.owner);
            }

            return memoryStream.ToArray(); ;
        }

        byte[,] convertStateToField(GTMillGameSpace state)
        {
            byte[,] field = malomField;
            foreach (KeyValuePair<GTMillPosition, GTMillGameElement> kv in state)
            {
                GTMillPosition p = kv.Key;
                int row = (3-p.z) * p.x + p.z;
                int column = (3-p.z) * p.y + p.z;

                field[row, column] = kv.Value.owner == _Logic.nextPlayer.id ? (byte)0 : (byte)1;
            }

            return field;
        }

        public GTGameSpaceInterface<GTMillGameElement, GTMillPosition> getNextState()
        {
            GTMillGameStateGenerator _generator = (GTMillGameStateGenerator)_Logic.getStateGenerator();
            GTMillGameStateHash _hash = (GTMillGameStateHash)_Logic.getStateHash();
            GTMillGameSpace _state = (GTMillGameSpace)_Logic.getCurrentState();

            switch (AIName)
            {
                case "RandomAi":
                    
                    return (GTGameSpaceInterface<GTMillGameElement, GTMillPosition>)_RandomAI.calculateNextStep(_state, _generator, _hash).Result;
                case "CorrectAi":
                    return (GTGameSpaceInterface<GTMillGameElement, GTMillPosition>)_CorrectAI.calculateNextStep(_state, _generator, _hash).Result;
                case "AlphaBetaAi":
                    return (GTGameSpaceInterface<GTMillGameElement, GTMillPosition>)_AlphaBetaAI.calculateNextStep(_state, _generator, _hash).Result;
                default:
                    return (GTGameSpaceInterface<GTMillGameElement, GTMillPosition>)_RandomAI.calculateNextStep(_state, _generator, _hash).Result;
            }
        }
    }
}
