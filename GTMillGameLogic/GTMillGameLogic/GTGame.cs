using System;
using System.Collections.Generic;
using System.IO;
using GTInterfacesLibrary;
using GTInterfacesLibrary.GameEvents;

namespace GTMillGameLogic
{
    public class GTGame : GTGameInterface
    {
        private GTPlatformManagerInterface _PlatformGameManager;
        private GTArtificialIntelligenceInterface<GTMillGameElement, GTMillPosition> AI;
        private GTGuiInterface _Gui;
        private readonly GTMillGameLogic _Logic;
        private GTMillPosition selectedPosition;

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

        public void RegisterArtificialIntelligence(String artificialIntelligenceName)
        {
            // this.AI = new;
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

            if (!currentState.hasElementAt(p)) {
                return;
            }

            GTMillGameElement e = _Logic.getCurrentState().elementAt(p);
            GTMillGameStep step = new GTMillGameStep(e, selectedPosition, p);

            bool goodStep = false;
            if (selectedPosition == null) {
                // selection
                if (e.owner == _Logic.nextPlayer.id) {
                    selectedPosition = p;
                }
            }
            else if (selectedPosition.Equals(GTMillPosition.Nowhere()))
            {
                // remove after Mill
                if (e.owner != _Logic.nextPlayer.id)
                {
                    selectedPosition = null;
                    goodStep = true;
                }
            }
            else
            {
                // step
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

            GTMillGameSpace state = (GTMillGameSpace)this.AI.calculateNextStep(_Logic.getCurrentState(), _Logic.getStateGenerator(), _Logic.getStateHash()).Result;
            _Logic.SetState(state);
            _Gui.SetField(convertStateToField(state));
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
                if (gameStateChangedEventArgs.GameType == GameType.Local)
                {
                    _Logic.addPlayer(new GTPlayer<GTMillGameElement, GTMillPosition>().playerWithRealUser(1));
                    _Logic.addPlayer(new GTPlayer<GTMillGameElement, GTMillPosition>().playerWithAI(this.AI, 2));
                }
                else if (gameStateChangedEventArgs.GameType == GameType.Ai)
                {
                    _Logic.addPlayer(new GTPlayer<GTMillGameElement, GTMillPosition>().playerWithAI(this.AI, 1));
                    _Logic.addPlayer(new GTPlayer<GTMillGameElement, GTMillPosition>().playerWithAI(this.AI, 2));
                }
                else if (gameStateChangedEventArgs.GameType == GameType.Online)
                {
                    _Logic.addPlayer(new GTPlayer<GTMillGameElement, GTMillPosition>().playerWithRealUser(1));
                    _Logic.addPlayer(new GTPlayer<GTMillGameElement, GTMillPosition>().playerWithRealUser(2));
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
    }
}
