using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GTInterfacesLibrary;

namespace GTMillGameLogic
{
	using TaskReturnType = List<GTGameSpaceInterface<GTMillGameElement, GTMillPosition>>;

	public class GTMillGameStateGenerator : GTGameStateGeneratorInterface<GTMillGameElement, GTMillPosition>
	{
        public Task<TaskReturnType> availableStatesFrom(GTGameSpaceInterface<GTMillGameElement, GTMillPosition> state, GTPlayerInterface<GTMillGameElement, GTMillPosition> player)
		{
			Task<TaskReturnType> task = Task<TaskReturnType>.Factory.StartNew (() => {
				
				List<GTMillGameStep> steps = new List<GTMillGameStep> ();
				foreach (KeyValuePair<GTMillPosition, GTMillGameElement> kv in state) {
					steps.AddRange(stepsFromPositionWithState(state as GTMillGameSpace, kv.Key));
				}

				TaskReturnType states = new TaskReturnType ();

				foreach (GTMillGameStep step in steps) {

                    if (GTMillGameMillDetector.detectMillOnPositionWithStateForUser(step.to, state as GTMillGameSpace, player.id))
                    {
                        throw new NotImplementedException();
                    }
					states.Add (state.stateWithStep (step));
				}
				return states;
			});

			return task;
		}

		private List<GTMillGameStep> stepsFromPositionWithState(GTMillGameSpace state, GTMillPosition position)
		{
			GTMillPosition[] positions = new GTMillPosition[6];
			for (int i = 0; i < 6; i++) {
				int[] coordinates = position.coordinates();
				coordinates [i / 2] += (i % 2 == 0 ? -1 : 1);
				positions [i] = new GTMillPosition (coordinates[0], coordinates[1], coordinates[2]);
			}

			List<GTMillPosition> availablePositions = new List<GTMillPosition> ();
			foreach (GTMillPosition p in positions) {
				if (!state.hasElementAt(p) && validPosition(p) && (position.z == p.z || !isCornerPosition (position))) {
					availablePositions.Add (p);
				}
			}

			List<GTMillGameStep> steps = new List<GTMillGameStep> (availablePositions.Count);
			foreach (GTMillPosition to in availablePositions) {
				steps.Add (new GTMillGameStep (state.elementAt (position), position, to));
			}

			return steps;
		}

		private bool validPosition(GTMillPosition p)
		{
			if (p.x < 0 || p.x > 2 || p.y < 0 || p.y > 2 || p.z < 0 || p.z > 2) {
				// position out of field
				return false;
			}

			if (p.x == 1 && p.y == 1) {
				// middle invalid position
				return false;
			}

			return true;
		}

		private bool isCornerPosition(GTMillPosition p) {
			return (p.x + p.y) % 2 == 0;
		}
	}
}
	