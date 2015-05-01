using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersGame.Logic
{
    public static class StepSupervisor
    {
        public static int BoardMinIndex = 0;
        public static int BoardMaxIndex = 7;

        public static bool IsValidStep(Step step)
        {
            if (IsValidMove(step) || IsValidCapture(step))
                return true;
            else
                return false;
        }

        private static bool IsValidMove(Step step)
        {
            if (IsDiagonal(step) && IsAdjacent(step) && !IsOccupied(step))
                return true;
            else
                return false;
        }

        private static bool IsValidCapture(Step step)
        {
            if (IsDiagonal(step) && IsAdjacentAdjacent(step) && !IsOccupied(step) && IsThereAnOpponentBetween(step))
                return true;
            else
                return false;
        }

        private static bool IsDiagonal(Step step)
        {
            throw new NotImplementedException();
        }

        private static bool IsAdjacent(Step step)
        {
            throw new NotImplementedException();
        }

        private static bool IsOccupied(Step step)
        {
            throw new NotImplementedException();
        }

        private static bool IsOpponent(Step step)
        {
            throw new NotImplementedException();
        }

        private static bool IsAdjacentAdjacent(Step step)
        {
            throw new NotImplementedException();
        }

        private static bool IsThereAnOpponentBetween(Step step)
        {
            throw new NotImplementedException();
        }
    }
}
