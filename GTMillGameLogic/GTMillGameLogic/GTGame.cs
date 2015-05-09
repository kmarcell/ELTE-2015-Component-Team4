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
        private GTArtificialIntelligenceInterface<GTGameSpaceElementInterface, GTPosition> AI;
        private GTGuiInterface _Gui;
        private readonly GTMillGameLogic _Logic;

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
            //
        }


        byte[,] malomBackGround = { 
                { 0, 9, 9, 4, 9, 9, 1 }, 
                { 10, 0, 9, 8, 9, 1, 10 },
                { 10, 10, 0, 6, 1, 10, 10 }, 
                { 7, 8, 5, 11, 7, 8, 5 },
                { 10, 10, 3, 4, 2, 10, 10 }, 
                { 10, 3, 9, 8, 9, 2, 10 },
                { 3, 9, 9, 6, 9, 9, 2 }};
        byte[,] malomField = { 
                { 0, 0, 0, 0, 0, 0, 0 }, 
                { 0, 0, 0, 0, 0, 0, 0 }, 
                { 0, 0, 0, 0, 0, 0, 0 }, 
                { 0, 0, 0, 0, 0, 0, 0 }, 
                { 0, 0, 0, 0, 0, 0, 0 }, 
                { 0, 0, 0, 0, 0, 0, 0 }, 
                { 0, 0, 0, 0, 0, 0, 0 }};


        public void RegisterGui(GTGuiInterface gui)
        {
            _Gui = gui;

            _Gui.SetFieldBackground(malomBackGround);
            _Gui.SetField(malomField);
            _Gui.FieldClicked += GuiOnFieldClicked;
        }

        private void GuiOnFieldClicked(GTGuiInterface gui, int row, int column)
        {
            malomField[row, column] = (byte)((malomField[row, column] + 1) % 3);
            gui.SetField(malomField);
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
    }
}
