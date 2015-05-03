﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTInterfacesLibrary;

namespace CheckersGame.Logic
{
    public class Logic: GTGameLogicInterface<Element, Position>
    {
        // properties
        public GameSpace state = new GameSpace();
        private string MyColor;
        private bool IsMyTurn;

		public Logic(string color)
		{
            init();
            state.MyColor = color;
            MyColor = color;
            if (MyColor == "white")
                IsMyTurn = true;
            else
                IsMyTurn = false;
		}

		// Input
		public void init()
		{
            StartingStateBuilder.BuildStartingState(state); 
		}

        public void updateGameSpace(GTGameStepInterface<Element, Position> step)
		{
			state.mutateStateWith(step);
		}

		// Output
		public Boolean isGameOver()
		{
            return state.Count() == 1;
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
    }
}
