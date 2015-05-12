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
            GTPlayerInterface<GTMillGameElement, GTMillPosition> player = new GTPlayer<GTMillGameElement, GTMillPosition>().playerWithRealUser(1);
            player.figuresInitial = 9;
            player.figuresRemaining = 0;
            player.figuresLost = 5;

            GTMillGameSpace state = new GTMillGameSpace();
            GTMillGameElement figure1 = new GTMillGameElement(1, 1, player.id);
            GTMillPosition position1 = new GTMillPosition(0, 0, 0);
            state.setElementAt(position1, figure1);

            GTMillGameStateGenerator generator = new GTMillGameStateGenerator();
            Task<List<GTGameSpaceInterface<GTMillGameElement, GTMillPosition>>> task = generator.availableStatesFrom(state, player);

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
            GTPlayerInterface<GTMillGameElement, GTMillPosition> player = new GTPlayer<GTMillGameElement, GTMillPosition>().playerWithRealUser(1);
            player.figuresInitial = 9;
            player.figuresRemaining = 0;
            player.figuresLost = 5;

            GTMillGameSpace state = new GTMillGameSpace();
            GTMillGameElement figure1 = new GTMillGameElement(1, 1, player.id);
            GTMillPosition position1 = new GTMillPosition(2, 1, 0);
            state.setElementAt(position1, figure1);

            GTMillGameStateGenerator generator = new GTMillGameStateGenerator();
            Task<List<GTGameSpaceInterface<GTMillGameElement, GTMillPosition>>> task = generator.availableStatesFrom(state, player);

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
        
        /**
         * Hash function tests
         **/

        /**
         * 2 pieces configurations
         **/
        [TestMethod()]
        public void TestCaseHashHorizontal2Pieces1()
        {
            GTMillGameStateHash hash = new GTMillGameStateHash();

            GTPlayer<GTMillGameElement, GTMillPosition> ownPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            ownPlayer.id = 1;
            ownPlayer.figuresRemaining = 3;

            GTPlayer<GTMillGameElement, GTMillPosition> opponentPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            opponentPlayer.id = 1;
            opponentPlayer.figuresRemaining = 3;

            //Top morrises
            GTMillGameSpace state1 = new GTMillGameSpace();
            state1.setElementAt(new GTMillPosition(0, 0, 0), new GTMillGameElement(1, 1, 1));
            state1.setElementAt(new GTMillPosition(1, 0, 0), new GTMillGameElement(2, 1, 1));

            int factor = hash.evaluateState(state1, ownPlayer);

            Assert.IsTrue(
                hash.twoPiecesConfiguration == 1 
                && hash.ownElements.Count == 2
            );
        }

        [TestMethod()]
        public void TestCaseHashHorizontal2Pieces2()
        {
            GTMillGameStateHash hash = new GTMillGameStateHash();

            GTPlayer<GTMillGameElement, GTMillPosition> ownPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            ownPlayer.id = 1;
            ownPlayer.figuresRemaining = 3;

            GTPlayer<GTMillGameElement, GTMillPosition> opponentPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            opponentPlayer.id = 1;
            opponentPlayer.figuresRemaining = 3;

            //Top morrises
            GTMillGameSpace state1 = new GTMillGameSpace();
            state1.setElementAt(new GTMillPosition(0, 0, 0), new GTMillGameElement(1, 1, 1));
            state1.setElementAt(new GTMillPosition(2, 0, 0), new GTMillGameElement(2, 1, 1));

            int factor = hash.evaluateState(state1, ownPlayer);

            Assert.IsTrue(
                hash.twoPiecesConfiguration == 1
                && hash.ownElements.Count == 2
            );
        }

        [TestMethod()]
        public void TestCaseHashHorizontal2Pieces3()
        {
            GTMillGameStateHash hash = new GTMillGameStateHash();

            GTPlayer<GTMillGameElement, GTMillPosition> ownPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            ownPlayer.id = 1;
            ownPlayer.figuresRemaining = 3;

            GTPlayer<GTMillGameElement, GTMillPosition> opponentPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            opponentPlayer.id = 1;
            opponentPlayer.figuresRemaining = 3;

            //Top morrises
            GTMillGameSpace state1 = new GTMillGameSpace();
            state1.setElementAt(new GTMillPosition(1, 0, 0), new GTMillGameElement(1, 1, 1));
            state1.setElementAt(new GTMillPosition(2, 0, 0), new GTMillGameElement(2, 1, 1));

            int factor = hash.evaluateState(state1, ownPlayer);

            Assert.IsTrue(
                hash.twoPiecesConfiguration == 1
                && hash.ownElements.Count == 2
            );
        }

        [TestMethod()]
        public void TestCaseHashVertical2Pieces1()
        {
            GTMillGameStateHash hash = new GTMillGameStateHash();

            GTPlayer<GTMillGameElement, GTMillPosition> ownPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            ownPlayer.id = 1;
            ownPlayer.figuresRemaining = 3;

            GTPlayer<GTMillGameElement, GTMillPosition> opponentPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            opponentPlayer.id = 1;
            opponentPlayer.figuresRemaining = 3;

            //Top morrises
            GTMillGameSpace state1 = new GTMillGameSpace();
            state1.setElementAt(new GTMillPosition(0, 0, 0), new GTMillGameElement(1, 1, 1));
            state1.setElementAt(new GTMillPosition(0, 1, 0), new GTMillGameElement(2, 1, 1));

            int factor = hash.evaluateState(state1, ownPlayer);

            Assert.IsTrue(
                hash.twoPiecesConfiguration == 1
                && hash.ownElements.Count == 2
            );
        }

        [TestMethod()]
        public void TestCaseHashVertical2Pieces2()
        {
            GTMillGameStateHash hash = new GTMillGameStateHash();

            GTPlayer<GTMillGameElement, GTMillPosition> ownPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            ownPlayer.id = 1;
            ownPlayer.figuresRemaining = 3;

            GTPlayer<GTMillGameElement, GTMillPosition> opponentPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            opponentPlayer.id = 1;
            opponentPlayer.figuresRemaining = 3;

            //Top morrises
            GTMillGameSpace state1 = new GTMillGameSpace();
            state1.setElementAt(new GTMillPosition(0, 0, 0), new GTMillGameElement(1, 1, 1));
            state1.setElementAt(new GTMillPosition(0, 2, 0), new GTMillGameElement(2, 1, 1));

            int factor = hash.evaluateState(state1, ownPlayer);

            Assert.IsTrue(
                hash.twoPiecesConfiguration == 1
                && hash.ownElements.Count == 2
            );
        }

        [TestMethod()]
        public void TestCaseHashVertical2Pieces3()
        {
            GTMillGameStateHash hash = new GTMillGameStateHash();

            GTPlayer<GTMillGameElement, GTMillPosition> ownPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            ownPlayer.id = 1;
            ownPlayer.figuresRemaining = 3;

            GTPlayer<GTMillGameElement, GTMillPosition> opponentPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            opponentPlayer.id = 1;
            opponentPlayer.figuresRemaining = 3;

            //Top morrises
            GTMillGameSpace state1 = new GTMillGameSpace();
            state1.setElementAt(new GTMillPosition(0, 1, 0), new GTMillGameElement(1, 1, 1));
            state1.setElementAt(new GTMillPosition(0, 2, 0), new GTMillGameElement(2, 1, 1));

            int factor = hash.evaluateState(state1, ownPlayer);

            Assert.IsTrue(
                hash.twoPiecesConfiguration == 1
                && hash.ownElements.Count == 2
            );
        }

        [TestMethod()]
        public void TestCaseHashDiagonal2Pieces1()
        {
            GTMillGameStateHash hash = new GTMillGameStateHash();

            GTPlayer<GTMillGameElement, GTMillPosition> ownPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            ownPlayer.id = 1;
            ownPlayer.figuresRemaining = 3;

            GTPlayer<GTMillGameElement, GTMillPosition> opponentPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            opponentPlayer.id = 1;
            opponentPlayer.figuresRemaining = 3;

            //Top morrises
            GTMillGameSpace state1 = new GTMillGameSpace();
            state1.setElementAt(new GTMillPosition(0, 0, 0), new GTMillGameElement(1, 1, 1));
            state1.setElementAt(new GTMillPosition(0, 0, 1), new GTMillGameElement(2, 1, 1));

            int factor = hash.evaluateState(state1, ownPlayer);

            Assert.IsTrue(
                hash.twoPiecesConfiguration == 0
                && hash.ownElements.Count == 2
            );
        }

        [TestMethod()]
        public void TestCaseHashDiagonal2Pieces2()
        {
            GTMillGameStateHash hash = new GTMillGameStateHash();

            GTPlayer<GTMillGameElement, GTMillPosition> ownPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            ownPlayer.id = 1;
            ownPlayer.figuresRemaining = 3;

            GTPlayer<GTMillGameElement, GTMillPosition> opponentPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            opponentPlayer.id = 1;
            opponentPlayer.figuresRemaining = 3;

            //Top morrises
            GTMillGameSpace state1 = new GTMillGameSpace();
            state1.setElementAt(new GTMillPosition(0, 0, 0), new GTMillGameElement(1, 1, 1));
            state1.setElementAt(new GTMillPosition(0, 0, 2), new GTMillGameElement(2, 1, 1));

            int factor = hash.evaluateState(state1, ownPlayer);

            Assert.IsTrue(
                hash.twoPiecesConfiguration == 0
                && hash.ownElements.Count == 2
            );
        }

        [TestMethod()]
        public void TestCaseHashDiagonal2Pieces3()
        {
            GTMillGameStateHash hash = new GTMillGameStateHash();

            GTPlayer<GTMillGameElement, GTMillPosition> ownPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            ownPlayer.id = 1;
            ownPlayer.figuresRemaining = 3;

            GTPlayer<GTMillGameElement, GTMillPosition> opponentPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            opponentPlayer.id = 1;
            opponentPlayer.figuresRemaining = 3;

            //Top morrises
            GTMillGameSpace state1 = new GTMillGameSpace();
            state1.setElementAt(new GTMillPosition(0, 0, 1), new GTMillGameElement(1, 1, 1));
            state1.setElementAt(new GTMillPosition(0, 0, 2), new GTMillGameElement(2, 1, 1));

            int factor = hash.evaluateState(state1, ownPlayer);

            Assert.IsTrue(
                hash.twoPiecesConfiguration == 0
                && hash.ownElements.Count == 2
            );
        }

        /**
         * 3 pieces configurations
         **/
        [TestMethod()]
        public void TestCaseHashHorizontal3Pieces1()
        {
            GTMillGameStateHash hash = new GTMillGameStateHash();

            GTPlayer<GTMillGameElement, GTMillPosition> ownPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            ownPlayer.id = 1;
            ownPlayer.figuresRemaining = 3;

            GTPlayer<GTMillGameElement, GTMillPosition> opponentPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            opponentPlayer.id = 1;
            opponentPlayer.figuresRemaining = 3;

            //Top morrises
            GTMillGameSpace state1 = new GTMillGameSpace();
            state1.setElementAt(new GTMillPosition(0, 0, 0), new GTMillGameElement(1, 1, 1));
            state1.setElementAt(new GTMillPosition(2, 0, 0), new GTMillGameElement(2, 1, 1));
            state1.setElementAt(new GTMillPosition(1, 0, 1), new GTMillGameElement(3, 1, 1));

            int factor = hash.evaluateState(state1, ownPlayer);

            Assert.IsTrue(
                hash.twoPiecesConfiguration == 1
                && hash.ownElements.Count == 3
                && hash.threePiecesConfiguration == 1
            );
        }

        [TestMethod()]
        public void TestCaseHashHorizontal3Pieces2()
        {
            GTMillGameStateHash hash = new GTMillGameStateHash();

            GTPlayer<GTMillGameElement, GTMillPosition> ownPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            ownPlayer.id = 1;
            ownPlayer.figuresRemaining = 3;

            GTPlayer<GTMillGameElement, GTMillPosition> opponentPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            opponentPlayer.id = 1;
            opponentPlayer.figuresRemaining = 3;

            //Top morrises
            GTMillGameSpace state1 = new GTMillGameSpace();
            state1.setElementAt(new GTMillPosition(0, 0, 0), new GTMillGameElement(1, 1, 1));
            state1.setElementAt(new GTMillPosition(2, 0, 0), new GTMillGameElement(2, 1, 1));
            state1.setElementAt(new GTMillPosition(0, 1, 0), new GTMillGameElement(3, 1, 1));

            int factor = hash.evaluateState(state1, ownPlayer);

            Assert.IsTrue(
                hash.twoPiecesConfiguration == 2
                && hash.ownElements.Count == 3
            );
        }

        [TestMethod()]
        public void TestCaseHashVertical3Pieces1()
        {
            GTMillGameStateHash hash = new GTMillGameStateHash();

            GTPlayer<GTMillGameElement, GTMillPosition> ownPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            ownPlayer.id = 1;
            ownPlayer.figuresRemaining = 3;

            GTPlayer<GTMillGameElement, GTMillPosition> opponentPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            opponentPlayer.id = 1;
            opponentPlayer.figuresRemaining = 3;

            //Top morrises
            GTMillGameSpace state1 = new GTMillGameSpace();
            state1.setElementAt(new GTMillPosition(2, 0, 0), new GTMillGameElement(1, 1, 1));
            state1.setElementAt(new GTMillPosition(2, 2, 0), new GTMillGameElement(2, 1, 1));
            state1.setElementAt(new GTMillPosition(2, 1, 1), new GTMillGameElement(3, 1, 1));

            int factor = hash.evaluateState(state1, ownPlayer);

            Assert.IsTrue(
                hash.twoPiecesConfiguration == 1
                && hash.ownElements.Count == 3
                && hash.threePiecesConfiguration == 1
            );
        }

        [TestMethod()]
        public void TestCaseHashVertical3Pieces2()
        {
            GTMillGameStateHash hash = new GTMillGameStateHash();

            GTPlayer<GTMillGameElement, GTMillPosition> ownPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            ownPlayer.id = 1;
            ownPlayer.figuresRemaining = 3;

            GTPlayer<GTMillGameElement, GTMillPosition> opponentPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            opponentPlayer.id = 1;
            opponentPlayer.figuresRemaining = 3;

            //Top morrises
            GTMillGameSpace state1 = new GTMillGameSpace();
            state1.setElementAt(new GTMillPosition(2, 0, 0), new GTMillGameElement(1, 1, 1));
            state1.setElementAt(new GTMillPosition(2, 2, 0), new GTMillGameElement(2, 1, 1));
            state1.setElementAt(new GTMillPosition(1, 2, 0), new GTMillGameElement(3, 1, 1));

            int factor = hash.evaluateState(state1, ownPlayer);

            Assert.IsTrue(
                hash.twoPiecesConfiguration == 2
                && hash.ownElements.Count == 3
            );
        }

        [TestMethod()]
        public void TestCaseHashDiagonal3Pieces1()
        {
            GTMillGameStateHash hash = new GTMillGameStateHash();

            GTPlayer<GTMillGameElement, GTMillPosition> ownPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            ownPlayer.id = 1;
            ownPlayer.figuresRemaining = 3;

            GTPlayer<GTMillGameElement, GTMillPosition> opponentPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            opponentPlayer.id = 1;
            opponentPlayer.figuresRemaining = 3;

            //Top morrises
            GTMillGameSpace state1 = new GTMillGameSpace();
            state1.setElementAt(new GTMillPosition(2, 1, 2), new GTMillGameElement(1, 1, 1));
            state1.setElementAt(new GTMillPosition(2, 1, 0), new GTMillGameElement(2, 1, 1));
            state1.setElementAt(new GTMillPosition(2, 0, 1), new GTMillGameElement(3, 1, 1));

            int factor = hash.evaluateState(state1, ownPlayer);

            Assert.IsTrue(
                hash.twoPiecesConfiguration == 1
                && hash.threePiecesConfiguration == 1
                && hash.ownElements.Count == 3
            );
        }

        [TestMethod()]
        public void TestCaseHashDiagonal3Pieces2()
        {
            GTMillGameStateHash hash = new GTMillGameStateHash();

            GTPlayer<GTMillGameElement, GTMillPosition> ownPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            ownPlayer.id = 1;
            ownPlayer.figuresRemaining = 3;

            GTPlayer<GTMillGameElement, GTMillPosition> opponentPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            opponentPlayer.id = 1;
            opponentPlayer.figuresRemaining = 3;

            //Top morrises
            GTMillGameSpace state1 = new GTMillGameSpace();
            state1.setElementAt(new GTMillPosition(2, 1, 2), new GTMillGameElement(1, 1, 1));
            state1.setElementAt(new GTMillPosition(2, 1, 0), new GTMillGameElement(2, 1, 1));
            state1.setElementAt(new GTMillPosition(2, 0, 2), new GTMillGameElement(3, 1, 1));

            int factor = hash.evaluateState(state1, ownPlayer);

            Assert.IsTrue(
                hash.twoPiecesConfiguration == 2
                && hash.ownElements.Count == 3
            );
        }

        /**
         * Blocked opponent
         **/
        [TestMethod()]
        public void TestCaseHashBlockedOpponent1()
        {
            GTMillGameStateHash hash = new GTMillGameStateHash();

            GTPlayer<GTMillGameElement, GTMillPosition> ownPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            ownPlayer.id = 1;
            ownPlayer.figuresRemaining = 3;

            GTPlayer<GTMillGameElement, GTMillPosition> opponentPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            opponentPlayer.id = 1;
            opponentPlayer.figuresRemaining = 3;

            //Top morrises
            GTMillGameSpace state1 = new GTMillGameSpace();
            state1.setElementAt(new GTMillPosition(0, 0, 0), new GTMillGameElement(1, 1, 1));
            state1.setElementAt(new GTMillPosition(1, 0, 0), new GTMillGameElement(2, 1, 2));
            state1.setElementAt(new GTMillPosition(2, 0, 0), new GTMillGameElement(3, 1, 1));
            state1.setElementAt(new GTMillPosition(1, 0, 1), new GTMillGameElement(4, 1, 1));

            int factor = hash.evaluateState(state1, ownPlayer);

            Assert.IsTrue(
                hash.opponentElements.Count == 1
                && hash.ownElements.Count == 3
                && hash.blockedOpponents == 1
            );
        }

        [TestMethod()]
        public void TestCaseHashBlockedOpponent2()
        {
            GTMillGameStateHash hash = new GTMillGameStateHash();

            GTPlayer<GTMillGameElement, GTMillPosition> ownPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            ownPlayer.id = 1;
            ownPlayer.figuresRemaining = 3;

            GTPlayer<GTMillGameElement, GTMillPosition> opponentPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            opponentPlayer.id = 1;
            opponentPlayer.figuresRemaining = 3;

            //Top morrises
            GTMillGameSpace state1 = new GTMillGameSpace();
            state1.setElementAt(new GTMillPosition(1, 0, 0), new GTMillGameElement(1, 1, 1));
            state1.setElementAt(new GTMillPosition(2, 0, 0), new GTMillGameElement(2, 1, 2));
            state1.setElementAt(new GTMillPosition(2, 1, 0), new GTMillGameElement(3, 1, 1));

            int factor = hash.evaluateState(state1, ownPlayer);

            Assert.IsTrue(
                hash.opponentElements.Count == 1
                && hash.ownElements.Count == 2
                && hash.blockedOpponents == 1
            );
        }

        /**
         * Horizontal morrises
         **/
        [TestMethod()]
        public void TestCaseHashHorizontalMorrises1()
        {
            GTMillGameStateHash hash = new GTMillGameStateHash();

            GTPlayer<GTMillGameElement, GTMillPosition> ownPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            ownPlayer.id = 1;
            ownPlayer.figuresRemaining = 3;

            GTPlayer<GTMillGameElement, GTMillPosition> opponentPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            opponentPlayer.id = 1;
            opponentPlayer.figuresRemaining = 3;
            
            //Top morrises
            GTMillGameSpace state1 = new GTMillGameSpace();
            state1.setElementAt(new GTMillPosition(0, 0, 0), new GTMillGameElement(1, 1, 1));
            state1.setElementAt(new GTMillPosition(1, 0, 0), new GTMillGameElement(2, 1, 1));
            state1.setElementAt(new GTMillPosition(2, 0, 0), new GTMillGameElement(3, 1, 1));

            int factor = hash.evaluateState(state1, ownPlayer);

            Assert.IsTrue(
                hash.morrises == 1
                && hash.ownElements.Count == 3
            );
        }

        [TestMethod()]
        public void TestCaseHashHorizontalMorrises2()
        {
            GTMillGameStateHash hash = new GTMillGameStateHash();

            GTPlayer<GTMillGameElement, GTMillPosition> ownPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            ownPlayer.id = 1;
            ownPlayer.figuresRemaining = 3;

            GTPlayer<GTMillGameElement, GTMillPosition> opponentPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            opponentPlayer.id = 1;
            opponentPlayer.figuresRemaining = 3;

            GTMillGameSpace state2 = new GTMillGameSpace();
            state2.setElementAt(new GTMillPosition(0, 0, 1), new GTMillGameElement(4, 1, 1));
            state2.setElementAt(new GTMillPosition(1, 0, 1), new GTMillGameElement(5, 1, 1));
            state2.setElementAt(new GTMillPosition(2, 0, 1), new GTMillGameElement(6, 1, 1));

            int factor = hash.evaluateState(state2, ownPlayer);

            Assert.IsTrue(
                hash.morrises == 1
                && hash.ownElements.Count == 3
            );

            GTMillGameSpace state3 = new GTMillGameSpace();
            state3.setElementAt(new GTMillPosition(0, 0, 2), new GTMillGameElement(7, 1, 1));
            state3.setElementAt(new GTMillPosition(1, 0, 2), new GTMillGameElement(8, 1, 1));
            state3.setElementAt(new GTMillPosition(2, 0, 2), new GTMillGameElement(9, 1, 1));

            int factor2 = hash.evaluateState(state3, ownPlayer);

            Assert.IsTrue(
                hash.morrises == 1
                && hash.ownElements.Count == 3
            );
        }

        [TestMethod()]
        public void TestCaseHashHorizontalMorrises3()
        {
            GTMillGameStateHash hash = new GTMillGameStateHash();

            GTPlayer<GTMillGameElement, GTMillPosition> ownPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            ownPlayer.id = 1;
            ownPlayer.figuresRemaining = 3;

            GTPlayer<GTMillGameElement, GTMillPosition> opponentPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            opponentPlayer.id = 1;
            opponentPlayer.figuresRemaining = 3;

            //Bottom morrises
            GTMillGameSpace state4 = new GTMillGameSpace();
            state4.setElementAt(new GTMillPosition(0, 2, 0), new GTMillGameElement(10, 1, 1));
            state4.setElementAt(new GTMillPosition(1, 2, 0), new GTMillGameElement(11, 1, 1));
            state4.setElementAt(new GTMillPosition(2, 2, 0), new GTMillGameElement(12, 1, 1));

            int factor2 = hash.evaluateState(state4, ownPlayer);

            Assert.IsTrue(
                hash.morrises == 1
                && hash.ownElements.Count == 3
            );
        }

        [TestMethod()]
        public void TestCaseHashHorizontalMorrises4()
        {
            GTMillGameStateHash hash = new GTMillGameStateHash();

            GTPlayer<GTMillGameElement, GTMillPosition> ownPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            ownPlayer.id = 1;
            ownPlayer.figuresRemaining = 3;

            GTPlayer<GTMillGameElement, GTMillPosition> opponentPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            opponentPlayer.id = 1;
            opponentPlayer.figuresRemaining = 3;

            GTMillGameSpace state5 = new GTMillGameSpace();
            state5.setElementAt(new GTMillPosition(0, 2, 1), new GTMillGameElement(13, 1, 1));
            state5.setElementAt(new GTMillPosition(1, 2, 1), new GTMillGameElement(14, 1, 1));
            state5.setElementAt(new GTMillPosition(2, 2, 1), new GTMillGameElement(15, 1, 1));

            int factor2 = hash.evaluateState(state5, ownPlayer);

            Assert.IsTrue(
                hash.morrises == 1
                && hash.ownElements.Count == 3
            );
        }

        [TestMethod()]
        public void TestCaseHashHorizontalMorrises5()
        {
            GTMillGameStateHash hash = new GTMillGameStateHash();

            GTPlayer<GTMillGameElement, GTMillPosition> ownPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            ownPlayer.id = 1;
            ownPlayer.figuresRemaining = 3;

            GTPlayer<GTMillGameElement, GTMillPosition> opponentPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            opponentPlayer.id = 1;
            opponentPlayer.figuresRemaining = 3;

            GTMillGameSpace state6 = new GTMillGameSpace();
            state6.setElementAt(new GTMillPosition(0, 2, 2), new GTMillGameElement(16, 1, 1));
            state6.setElementAt(new GTMillPosition(1, 2, 2), new GTMillGameElement(17, 1, 1));
            state6.setElementAt(new GTMillPosition(2, 2, 2), new GTMillGameElement(18, 1, 1));

            int factor2 = hash.evaluateState(state6, ownPlayer);

            Assert.IsTrue(
                hash.morrises == 1
                && hash.ownElements.Count == 3
            );
        }

        [TestMethod()]
        public void TestCaseHashHorizontalMorrises6()
        {
            GTMillGameStateHash hash = new GTMillGameStateHash();

            GTPlayer<GTMillGameElement, GTMillPosition> ownPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            ownPlayer.id = 1;
            ownPlayer.figuresRemaining = 3;

            GTPlayer<GTMillGameElement, GTMillPosition> opponentPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            opponentPlayer.id = 1;
            opponentPlayer.figuresRemaining = 3;

            //Opponent player
            GTMillGameSpace state8 = new GTMillGameSpace();
            state8.setElementAt(new GTMillPosition(0, 2, 0), new GTMillGameElement(10, 1, 2));
            state8.setElementAt(new GTMillPosition(1, 2, 0), new GTMillGameElement(11, 1, 2));
            state8.setElementAt(new GTMillPosition(2, 2, 0), new GTMillGameElement(12, 1, 2));

            int factor2 = hash.evaluateState(state8, ownPlayer);

            Assert.IsTrue(
                hash.opponentElements.Count == 3
            );
        }

        [TestMethod()]
        public void TestCaseWinningConfiguration()
        {
            GTMillGameStateHash hash = new GTMillGameStateHash();

            GTPlayer<GTMillGameElement, GTMillPosition> ownPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            ownPlayer.id = 1;
            ownPlayer.figuresRemaining = 3;

            GTPlayer<GTMillGameElement, GTMillPosition> opponentPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            opponentPlayer.id = 1;
            opponentPlayer.figuresRemaining = 3;

            //Own player
            GTMillGameSpace state8 = new GTMillGameSpace();
            state8.setElementAt(new GTMillPosition(0, 2, 0), new GTMillGameElement(10, 1, 1));
            state8.setElementAt(new GTMillPosition(1, 2, 0), new GTMillGameElement(11, 1, 1));
            state8.setElementAt(new GTMillPosition(2, 2, 0), new GTMillGameElement(12, 1, 1));

            int factor = hash.evaluateState(state8, ownPlayer);

            Assert.IsTrue(
                factor
                    == Int32.MaxValue
            );
        }

        [TestMethod()]
        public void TestCaseLosingConfiguration()
        {
            GTMillGameStateHash hash = new GTMillGameStateHash();

            GTPlayer<GTMillGameElement, GTMillPosition> ownPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            ownPlayer.id = 1;
            ownPlayer.figuresRemaining = 3;

            GTPlayer<GTMillGameElement, GTMillPosition> opponentPlayer = new GTPlayer<GTMillGameElement, GTMillPosition>();
            opponentPlayer.id = 1;
            opponentPlayer.figuresRemaining = 3;

            //Own player
            GTMillGameSpace state8 = new GTMillGameSpace();
            state8.setElementAt(new GTMillPosition(0, 2, 0), new GTMillGameElement(10, 1, 2));
            state8.setElementAt(new GTMillPosition(1, 2, 0), new GTMillGameElement(11, 1, 2));
            state8.setElementAt(new GTMillPosition(2, 2, 0), new GTMillGameElement(12, 1, 2));

            int factor = hash.evaluateState(state8, ownPlayer);

            Assert.IsTrue(
                factor
                    == Int32.MinValue
            );
        }
    }
}
