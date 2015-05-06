using System;
using GTInterfacesLibrary;
using GTInterfacesLibrary.GameEvents;
using System.Collections.Generic;
using System.IO;

namespace GTMillGameLogic
{
	public class GTMillGameLogic : GTGameLogicInterface<GTMillGameElement, GTMillPosition>
	{
		// properties
		private GTMillGameSpace _state = new GTMillGameSpace();

		public GTMillGameLogic ()
        {
            Name = "MillGame";
            Id = 1;
            Description = "MillGame";
		}

		// Input
		public void init()
		{
		}

		public void updateGameSpace (GTGameStepInterface<GTMillGameElement, GTMillPosition> step)
		{
			this._state.mutateStateWith (step);
		}

		// Output
		public Boolean isGameOver()
		{
			return false;
		}

		public GTGameSpaceInterface<GTMillGameElement, GTMillPosition> getCurrentState()
		{
			return _state;
		}

		public GTGameStateGeneratorInterface<GTMillGameElement, GTMillPosition> getStateGenerator()
		{
			return new GTMillGameStateGenerator();
		}

		public GTGameStateHashInterface<GTMillGameElement, GTMillPosition> getStateHash()
		{
			return new GTMillGameStateHash();
		}

	    public event EventHandler<GameStateChangedEventArgs> SendGameStateChangedEventArg;
	    public void SendGameState(GameStateChangedEventArgs currentGameStateChangedEventArgs)
	    {
	        throw new NotImplementedException();
	    }

	    public void RegisterGameManager(GTPlatformManagerInterface platformGameManager)
	    {
	        throw new NotImplementedException();
	    }

        public void RegisterArtificialIntelligence(IGTArtificialIntelligenceInterface artificialIntelligence)
	    {
	        throw new NotImplementedException();
	    }

	    public void RegisterGui(GTGuiInterface gui)
        {
            // register
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

            _state = newState;
	    }

	    public byte[] SaveGame()
	    {
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(memoryStream);
            foreach (KeyValuePair<GTMillPosition, GTMillGameElement> kv in _state)
            {
                writer.Write(kv.Key.x);
                writer.Write(kv.Key.y);
                writer.Write(kv.Key.z);
                writer.Write(kv.Value.id);
                writer.Write(kv.Value.type);
                writer.Write(kv.Value.owner);
            }

            return memoryStream.ToArray();;
	    }
	}
}

