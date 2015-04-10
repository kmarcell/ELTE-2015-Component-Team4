using NUnit.Framework;
using System;
using System.Collections.Generic;
using GTMillGameLogic;
using GTInterfacesLibrary;

namespace GTMillGameLogicTests
{
	[TestFixture ()]
	public class Test
	{
		[Test ()]
		public void TestCaseSetInitialStateIsEmptyAndNotGameOver ()
		{
			// Test no elements
			GTMillGameSpace gamespace = new GTMillGameSpace ();
			int count = 0;
			foreach ( KeyValuePair<GTMillPosition, GTMillGameElement> element in gamespace) {
				// zero elements should exist
				count++;
			}
			Assert.AreEqual (0, count);

			// Test no game over
			GTMillGameLogic.GTMillGameLogic logic = new GTMillGameLogic.GTMillGameLogic();
			Assert.False(logic.isGameOver());
		}

		[Test ()]
		public void TestCaseGameSpaceAddFigureToBoard ()
		{
			// given
			GTMillGameSpace gamespace = new GTMillGameSpace ();
			// id must be > 0
			GTMillPosition p1 = new GTMillPosition (0, 0, 0);
			GTMillGameElement e1 = new GTMillGameElement(1, 0, 0);

			GTMillPosition p2 = new GTMillPosition (0, 0, 1);
			GTMillGameElement e2 = new GTMillGameElement(2, 1, 1);

			// when
			gamespace.setElementAt (p1, e1);
			gamespace.setElementAt (p2, e2);

			// then
			Assert.AreEqual(e1, gamespace.elementAt(p1));
			Assert.AreEqual(e2, gamespace.elementAt(p2));
		}

		[Test ()]
		public void TestCaseGameSpaceTakeAStep ()
		{
			// given
			GTMillGameSpace gamespace = new GTMillGameSpace ();
			GTMillGameElement figure1 = new GTMillGameElement (1, 1, 1);
			GTMillPosition position1 = new GTMillPosition (0, 0, 0); // top left corner of the mill board on most outer level
			gamespace.setElementAt(position1, figure1);

			GTMillPosition position2 = new GTMillPosition(1, 0, 0); // top center position on most outer level

			GTMillGameStep step = new GTMillGameStep (figure1, position1, position2);

			// when
			gamespace.mutateStateWith(step);

			// then
			Assert.AreEqual(false, gamespace.hasElementAt(position1));
			Assert.AreEqual(true, gamespace.hasElementAt(position2));
			Assert.AreEqual(figure1, gamespace.elementAt(position2));
		}
	}
}

