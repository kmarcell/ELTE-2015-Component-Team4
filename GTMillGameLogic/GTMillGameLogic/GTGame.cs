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
            SendGameStateChangedEventArg(this, eventArgs);
        }

        public void RegisterGameManager(GTPlatformManagerInterface platformGameManager)
        {
            _PlatformGameManager = platformGameManager;
            _PlatformGameManager.SendGameStateChangedEvent += RecieveGameState;
        }

        public void RegisterArtificialIntelligence(IGTArtificialIntelligenceInterface artificialIntelligence)
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

        //byte[,] malomField { 
        //    get {
        byte[,] malomField =
        { 
            { 2, 2, 2, 2, 2, 2, 2 }, 
            { 2, 2, 2, 2, 2, 2, 2 }, 
            { 2, 2, 2, 2, 2, 2, 2 },
            { 2, 2, 2, 2, 2, 2, 2 },
            { 2, 2, 2, 2, 2, 2, 2 },
            { 2, 2, 2, 2, 2, 2, 2 },
            { 2, 2, 2, 2, 2, 2, 2 }};

        //        return malomField;
        //    }
        //}


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

            if (selectedPosition == null) {
                selectedPosition = convertFromUI(row, column);
                return;
            }

            GTMillPosition p = convertFromUI(row, column);
            GTMillGameElement e = _Logic.getCurrentState().elementAt(p);
            GTMillGameSpace currentState = (GTMillGameSpace)_Logic.getCurrentState();
            GTMillGameStep step = new GTMillGameStep(e, selectedPosition, p); 
            GTMillGameSpace newState = (GTMillGameSpace)currentState.stateWithStep(step);
            selectedPosition = null;

            List<GTGameSpaceInterface<GTMillGameElement, GTMillPosition>> availableStates = ((GTMillGameStateGenerator)_Logic.getStateGenerator()).availableStatesFrom(currentState, _Logic.nextPlayer, false).Result;
            bool goodStep = false;
            foreach (GTMillGameSpace state in availableStates)
            {
                if (newState.Equals(state)) {
                    goodStep = true;
                }
            }

            if (!goodStep)
            {
                return;
            }

            _Logic.updateGameSpace(step);

            malomField = convertStateToField(newState);
            _Gui.SetField(malomField);

            if (_Logic.isGameOver())
            {
                GameStateChangedEventArgs _event = new GameStateChangedEventArgs();
                _event.GameState = this.SaveGame();
                _event.IsMyTurn = false;
                _event.GamePhase = GamePhase.Ended;
                _event.IsWon = true;

                SendGameState(_event);
                return;
            }
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

                field[row, column] = (byte)kv.Value.id;
            }

            return field;
        }
    }
}
