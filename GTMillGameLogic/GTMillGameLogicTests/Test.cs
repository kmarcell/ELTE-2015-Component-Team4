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
			GTMillGameSpace gamespace = new GTMillGameSpace ();
			int count = 0;
			foreach ( KeyValuePair<GTMillPosition, GTGameSpaceElementInterface> element in gamespace) {
				// zero element should exist
				count++;
			}
			Assert.AreEqual (0, count);
		}
	}
}

