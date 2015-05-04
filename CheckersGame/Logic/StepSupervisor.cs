using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CheckersGame.Logic
{
    public static class StepSupervisor
    {
        public static int BoardMinIndex = 0;
        public static int BoardMaxIndex = 7;
        private static GameSpace CurrentState;

        public static void RefreshState(GameSpace state)
        {
            CurrentState = state;
        }

        public static bool IsValidStep(GameSpace state, Step step)
        {
            CurrentState = state;

            if (step.element == null)
                return false;

            if (state.nextPlayer != step.element.owner)
                return false;

            if (!state.hasElementAt(step.from))
                return false;

            if (CanCapture())
            {
                if (IsValidCapture(step))
                    return true;
                else
                    return false;
            }
            else
            {
                if (IsValidMove(step))
                    return true;
                else
                    return false;
            }
        }

        public static bool CanCapture()
        {
            var myElements = CurrentState.GetElements().Where(x => IsMine(x.Value));
            var opponentElements = CurrentState.GetElements().Where(x => !IsMine(x.Value));

            foreach (KeyValuePair<Position, Element> mine in myElements)
            {
                foreach (KeyValuePair<Position, Element> opponent in opponentElements)
                {
                    if (CanCaptureElement(mine, opponent))
                        return true;
                }
            }

            return false;
        }

        private static bool CanCaptureElement(KeyValuePair<Position, Element> mine, KeyValuePair<Position, Element> opponent)
        {
            if (!IsDiagonal(mine.Key, opponent.Key) || !IsAdjacent(mine.Key, opponent.Key))
                return false;

            Position jumpPosition = PositionBehind(mine.Key, opponent.Key);

            if (jumpPosition == null)
                return false;

            Step step = new Step(mine.Value, mine.Key, jumpPosition);
            if (!IsOccupied(step) && IsForward(step))
                return true;
            else
                return false;
        }

        private static bool IsValidMove(Step step)
        {
            if (IsDiagonal(step) && IsForward(step) && IsAdjacent(step) && !IsOccupied(step))
                return true;
            else
                return false;
        }

        public static bool IsValidCapture(Step step)
        {
            if (!IsDiagonal(step) || !IsForward(step) || !IsAdjacentAdjacent(step) || IsOccupied(step))
                return false;

            Position posBetween = PositionBetween(step.from, step.to);
            if (IsThereAnOpponent(posBetween))
                return true;
            else
                return false;
        }

        public static Position CapturedElementPos(Step step)
        {
            if (!IsDiagonal(step) || !IsForward(step) || !IsAdjacentAdjacent(step) || IsOccupied(step))
                return null;

            Position posBetween = PositionBetween(step.from, step.to);
            if (IsThereAnOpponent(posBetween))
                return posBetween;
            else
                return null;
        }

        private static bool IsDiagonal(Step step)
        {
            if (Math.Abs(step.from.x - step.to.x) == Math.Abs(step.from.y - step.to.y))
                return true;
            else
                return false;
        }

        private static bool IsDiagonal(Position pos1, Position pos2)
        {
            if (Math.Abs(pos1.x - pos2.x) == Math.Abs(pos1.y - pos2.y))
                return true;
            else
                return false;
        }

        private static bool IsForward(Step step)
        {
            if (step.element.owner == 1)
            {
                if (step.from.x < step.to.x)
                    return true;
                else
                    return false;
            }
            else
            {
                if (step.from.x > step.to.x)
                    return true;
                else
                    return false;
            }
        }

        private static bool IsAdjacent(Step step)
        {
            if (Math.Abs(step.from.x - step.to.x) == 1 && Math.Abs(step.from.y - step.to.y) == 1)
                return true;
            else
                return false;
        }

        private static bool IsAdjacent(Position pos1, Position pos2)
        {
            if (Math.Abs(pos1.x - pos2.x) == 1 && Math.Abs(pos1.y - pos2.y) == 1)
                return true;
            else
                return false;
        }

        private static bool IsAdjacentAdjacent(Step step)
        {
            if (Math.Abs(step.from.x - step.to.x) == 2 && Math.Abs(step.from.y - step.to.y) == 2)
                return true;
            else
                return false;
        }

        private static bool IsOccupied(Step step)
        {
            if (CurrentState.hasElementAt(step.to))
                return true;
            else
                return false;
        }

        private static bool IsThereAnOpponent(Position pos)
        {
            if (pos == null)
                return false;

            if (!CurrentState.hasElementAt(pos))
                return false;

            if (!IsMine(CurrentState.elementAt(pos)))
                return true;
            else
                return false;
        }

        private static bool IsMine(Element e)
        {
            if (CurrentState.nextPlayer == e.owner)
                return true;
            else
                return false;
        }

        private static Position PositionBehind(Position from, Position to)
        {
            if (to.x == 0 || to.x == 7 || to.y == 0 || to.y == 7)
                return null;

            int xMove = to.x - from.x;
            int yMove = to.y - from.y;

            return new Position(to.x + xMove, to.y + yMove);
        }

        private static Position PositionBetween(Position from, Position to)
        {
            int xMove = to.x - from.x;
            int yMove = to.y - from.y;

            return new Position(to.x - xMove/2, to.y - yMove/2);
        }
    }
}
