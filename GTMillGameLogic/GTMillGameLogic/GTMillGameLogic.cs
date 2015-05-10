using GTInterfacesLibrary;
using GTInterfacesLibrary.GameEvents;
using GTInterfacesLibrary.GameEvents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GTMillGameLogic
{
	public class GTMillGameLogic : GTGameLogicInterface<GTMillGameElement, GTMillPosition>
	{
		// properties
		private GTMillGameSpace _state = new GTMillGameSpace();
		private List<GTPlayerInterface<GTMillGameElement, GTMillPosition>> _players = new List<GTPlayerInterface<GTMillGameElement, GTMillPosition>> ();
		int _nextPlayer = 0;

        // Input
		public void init()
		{
		}

        public void addPlayer(GTPlayerInterface<GTMillGameElement, GTMillPosition> player)
        {
            _players.Add(player);
        }

        public void updateGameSpace(GTGameStepInterface<GTMillGameElement, GTMillPosition> step)
        {
            this._state.mutateStateWith(step);
        }

		// Output
		public Boolean isGameOver()
		{
            GTPlayerInterface<GTMillGameElement, GTMillPosition> player = this.nextPlayer;
            if (player.figuresInitial - player.figuresLost <= 2)
            {
                // only 2 figures left
                return true;
            }

            int choises = this.getStateGenerator().availableStatesFrom(_state, player).Result.Count;
            if (choises == 0)
            {
                // no more steps
                return true;
            }

            return false;

		}

		public GTGameSpaceInterface<GTMillGameElement, GTMillPosition> getCurrentState()
		{
			return _state;
		}

	    public void SetState(GTMillGameSpace state)
	    {
	        _state = state;
	    }

		public GTGameStateGeneratorInterface<GTMillGameElement, GTMillPosition> getStateGenerator()
		{
			return new GTMillGameStateGenerator();
		}

		public GTGameStateHashInterface<GTMillGameElement, GTMillPosition> getStateHash()
		{
			return new GTMillGameStateHash();
		}

		public GTPlayerInterface<GTMillGameElement, GTMillPosition> nextPlayer {
			get {
				return _players [_nextPlayer];
			}
		}

        public void playerDidStep()
        {
            _nextPlayer = _nextPlayer++ % _players.Count;
        }

		public int gamePhase {
			get {

				GTPlayerInterface<GTMillGameElement, GTMillPosition> player = _players[0];
				if (player.figuresRemaining > 0) {
					// first phase
					return 0;

				} else if (player.figuresInitial - player.figuresLost > 3) {
					// second phase
					return 1;

				} else {
					// third phase
					return 2;
				}
			}
		}
	}
}

