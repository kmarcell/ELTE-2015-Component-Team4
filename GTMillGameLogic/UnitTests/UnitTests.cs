using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;
using GTMillGameLogic;
using GTInterfacesLibrary;

namespace UnitTests
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void TestCaseSetInitialStateIsEmptyAndNotGameOver()
        {
            // Test no elements
            GTMillGameSpace gamespace = new GTMillGameSpace();
            int count = 0;
            foreach (KeyValuePair<GTMillPosition, GTMillGameElement> element in gamespace)
            {
                // zero elements should exist
                count++;
            }
            Assert.AreEqual(0, count);

            // Test no game over
            GTMillGameLogic.GTMillGameLogic logic = new GTMillGameLogic.GTMillGameLogic();
            Assert.IsFalse(logic.isGameOver());
        }

        [TestMethod]
        public void TestCaseGameSpaceAddFigureToBoard()
        {
            // given
            GTMillGameSpace gamespace = new GTMillGameSpace();
            // id must be > 0
            GTMillPosition p1 = new GTMillPosition(0, 0, 0);
            GTMillGameElement e1 = new GTMillGameElement(1, 0, 0);

            GTMillPosition p2 = new GTMillPosition(0, 0, 1);
            GTMillGameElement e2 = new GTMillGameElement(2, 1, 1);

            // when
            gamespace.setElementAt(p1, e1);
            gamespace.setElementAt(p2, e2);

            // then
            Assert.AreEqual(e1, gamespace.elementAt(p1));
            Assert.AreEqual(e2, gamespace.elementAt(p2));
        }

        [TestMethod]
        public void TestCaseGameSpaceTakeAStep()
        {
            // given
            GTMillGameSpace gamespace = new GTMillGameSpace();
            GTMillGameElement figure1 = new GTMillGameElement(1, 1, 1);
            GTMillPosition position1 = new GTMillPosition(0, 0, 0); // top left corner of the mill board on most outer level
            gamespace.setElementAt(position1, figure1);

            GTMillPosition position2 = new GTMillPosition(1, 0, 0); // top center position on most outer level

            GTMillGameStep step = new GTMillGameStep(figure1, position1, position2);

            // when
            gamespace.mutateStateWith(step);

            // then
            Assert.AreEqual(false, gamespace.hasElementAt(position1));
            Assert.AreEqual(true, gamespace.hasElementAt(position2));
            Assert.AreEqual(figure1, gamespace.elementAt(position2));
        }

        [TestMethod()]
        public void TestCaseStateGeneratorFromTopLeftCorner()
        {

            // given
            GTMillGameSpace state = new GTMillGameSpace();
            GTMillGameElement figure1 = new GTMillGameElement(1, 1, 1);
            GTMillPosition position1 = new GTMillPosition(0, 0, 0);
            state.setElementAt(position1, figure1);

            GTMillGameStateGenerator generator = new GTMillGameStateGenerator();
            Task<List<GTGameSpaceInterface<GTMillGameElement, GTMillPosition>>> task = generator.availableStatesFrom(state);

            // when
            List<GTGameSpaceInterface<GTMillGameElement, GTMillPosition>> availableStates = task.Result;

            // then
            Assert.AreEqual(2, availableStates.Count);
            GTMillGameSpace newState = (GTMillGameSpace)availableStates[0];
            Assert.AreEqual(figure1, newState.elementAt(new GTMillPosition(1, 0, 0)));
            newState = (GTMillGameSpace)availableStates[1];
            Assert.AreEqual(figure1, newState.elementAt(new GTMillPosition(0, 1, 0)));
        }

        [TestMethod()]
        public void TestCaseStateGeneratorFromMostRightMiddle()
        {

            // given
            GTMillGameSpace state = new GTMillGameSpace();
            GTMillGameElement figure1 = new GTMillGameElement(1, 1, 1);
            GTMillPosition position1 = new GTMillPosition(2, 1, 0);
            state.setElementAt(position1, figure1);

            GTMillGameStateGenerator generator = new GTMillGameStateGenerator();
            Task<List<GTGameSpaceInterface<GTMillGameElement, GTMillPosition>>> task = generator.availableStatesFrom(state);

            // when
            List<GTGameSpaceInterface<GTMillGameElement, GTMillPosition>> availableStates = task.Result;

            // then
            Assert.AreEqual(3, availableStates.Count);
            GTMillGameSpace newState = (GTMillGameSpace)availableStates[0];
            Assert.AreEqual(figure1, newState.elementAt(new GTMillPosition(2, 0, 0)));
            newState = (GTMillGameSpace)availableStates[1];
            Assert.AreEqual(figure1, newState.elementAt(new GTMillPosition(2, 2, 0)));
            newState = (GTMillGameSpace)availableStates[2];
            Assert.AreEqual(figure1, newState.elementAt(new GTMillPosition(2, 1, 1)));
        }

        [TestMethod()]
        public void TestCaseMillDetectorHorizontal()
        {

            // given
            GTMillGameSpace state = new GTMillGameSpace();
            GTMillGameElement figure1 = new GTMillGameElement(1, 1, 1);
            GTMillGameElement figure2 = new GTMillGameElement(2, 1, 1);
            GTMillGameElement figure3 = new GTMillGameElement(3, 1, 1);

            GTMillPosition position1 = new GTMillPosition(0, 0, 0);
            GTMillPosition position2 = new GTMillPosition(1, 0, 0);
            GTMillPosition position3 = new GTMillPosition(2, 0, 0);

            // when
            state.setElementAt(position1, figure1);
            state.setElementAt(position2, figure2);
            state.setElementAt(position3, figure3);

            // then
            Assert.IsTrue(GTMillGameMillDetector.detectMillOnPositionWithStateForUser(position1, state, 1));
            Assert.IsTrue(GTMillGameMillDetector.detectMillOnPositionWithStateForUser(position2, state, 1));
            Assert.IsTrue(GTMillGameMillDetector.detectMillOnPositionWithStateForUser(position3, state, 1));
        }

        [TestMethod()]
        public void TestCaseMillDetectorVertical()
        {

            // given
            GTMillGameSpace state = new GTMillGameSpace();
            GTMillGameElement figure1 = new GTMillGameElement(1, 1, 1);
            GTMillGameElement figure2 = new GTMillGameElement(2, 1, 1);
            GTMillGameElement figure3 = new GTMillGameElement(3, 1, 1);

            GTMillPosition position1 = new GTMillPosition(2, 0, 0);
            GTMillPosition position2 = new GTMillPosition(2, 1, 0);
            GTMillPosition position3 = new GTMillPosition(2, 2, 0);

            // when
            state.setElementAt(position1, figure1);
            state.setElementAt(position2, figure2);
            state.setElementAt(position3, figure3);

            // then
            Assert.IsTrue(GTMillGameMillDetector.detectMillOnPositionWithStateForUser(position1, state, 1));
            Assert.IsTrue(GTMillGameMillDetector.detectMillOnPositionWithStateForUser(position2, state, 1));
            Assert.IsTrue(GTMillGameMillDetector.detectMillOnPositionWithStateForUser(position3, state, 1));
        }

        [TestMethod()]
        public void TestCaseMillDetectorCross()
        {

            // given
            GTMillGameSpace state = new GTMillGameSpace();
            GTMillGameElement figure1 = new GTMillGameElement(1, 1, 1);
            GTMillGameElement figure2 = new GTMillGameElement(2, 1, 1);
            GTMillGameElement figure3 = new GTMillGameElement(3, 1, 1);

            GTMillPosition position1 = new GTMillPosition(2, 2, 0);
            GTMillPosition position2 = new GTMillPosition(2, 2, 1);
            GTMillPosition position3 = new GTMillPosition(2, 2, 2);

            // when
            state.setElementAt(position1, figure1);
            state.setElementAt(position2, figure2);
            state.setElementAt(position3, figure3);

            // then
            Assert.IsTrue(GTMillGameMillDetector.detectMillOnPositionWithStateForUser(position1, state, 1));
            Assert.IsTrue(GTMillGameMillDetector.detectMillOnPositionWithStateForUser(position2, state, 1));
            Assert.IsTrue(GTMillGameMillDetector.detectMillOnPositionWithStateForUser(position3, state, 1));
        }
    }
}
