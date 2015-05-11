using System;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using GTInterfacesLibrary;
using GTInterfacesLibrary.GameEvents;
using System.Collections.Generic;

namespace CheckersGame.Logic
{
    public class Logic: GTGameLogicInterface<Element, Position>
    {
        private List<GTPlayerInterface<Element, Position>> _players = new List<GTPlayerInterface<Element, Position>>();
        private RandomAI AI;

		public Logic()
        {
            init();
		}

        // properties
        public GameSpace state = new GameSpace();

		// Input
		public void init()
		{
            StartingStateBuilder.BuildStartingState(state);
            StepSupervisor.RefreshState(state);
            AI = new RandomAI();
		}

        public void updateGameSpace(GTGameStepInterface<Element, Position> step)
		{
            if (StepSupervisor.IsValidStep(state, (Step)step))
            {
                state.mutateStateWith(step);
            }
		}

        public void addPlayer(GTPlayerInterface<Element, Position> player)
        {
            _players.Add(player);
            state.addPlayer(player);
        }

		// Output
		public Boolean isGameOver()
		{
            int whiteCount = state.Count(x => x.Value.owner == 1);
            int blackCount = state.Count(x => x.Value.owner == 0);
            return (whiteCount == 0 || blackCount == 0);
		}

        public GTGameSpaceInterface<Element, Position> getCurrentState()
		{
            return state;
		}

		public GTGameStateGeneratorInterface<Element, Position> getStateGenerator()
		{
			return new GameStateGenerator();
		}

		public GTGameStateHashInterface<Element, Position> getStateHash()
		{
			return new GameStateHash();
		}

        public void ChangePlayer()
        {
            state.changePlayer();
        }

        public GTPlayerInterface<Element, Position> nextPlayer
        {
            get { return _players[state.nextPlayer]; }
        }

        public int gamePhase
        {
            get { return 0; }
        }

        public GTGameSpaceInterface<Element, Position> getNextState()
        {
            return (GTGameSpaceInterface<Element, Position>)AI.calculateNextStep(
                state,
                getStateGenerator(),
                getStateHash()).Result;
        }
    }
}