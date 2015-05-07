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
			if (player.figuresRemaining > 0) {
				// first phase
				return availableStatesFirstPhase(state, player);

			} else if (player.figuresInitial - player.figuresLost > 3) {
				// second phase
				return availableStatesSteppingPhase (state, player);

			} else {
				// third phase
				return availableStatesThirdPhase (state, player);
			}
		}	

		private Task<TaskReturnType> availableStatesFirstPhase(GTGameSpaceInterface<GTMillGameElement, GTMillPosition> state, GTPlayerInterface<GTMillGameElement, GTMillPosition> player)
		{
			Task<TaskReturnType> task = Task<TaskReturnType>.Factory.StartNew (() => {
				TaskReturnType states = new TaskReturnType ();

				List<GTMillGameStep> steps = new List<GTMillGameStep> ();
				for (int x = 0; x < 3; ++x) {
					for (int y = 0; y < 3; ++y) {
						for (int z = 0; z < 3; ++z) {
							GTMillPosition p = new GTMillPosition(x, y ,z);
							if (!state.hasElementAt(p)) {
								int id = player.id * player.figuresLost + 1;
								GTMillGameElement element = new GTMillGameElement(id, 1, player.id);
								steps.Add(new GTMillGameStep(element, GTMillPosition.Nowhere(), p));
							}
						}
					}
				}

				return states;
			});
			return task;
		}

        private Task<TaskReturnType> availableStatesSteppingPhase(GTGameSpaceInterface<GTMillGameElement, GTMillPosition> state, GTPlayerInterface<GTMillGameElement, GTMillPosition> player)
		{
			Task<TaskReturnType> task = Task<TaskReturnType>.Factory.StartNew (() => {
				
				List<GTMillGameStep> steps = new List<GTMillGameStep> ();
				foreach (KeyValuePair<GTMillPosition, GTMillGameElement> kv in state) {
					if (kv.Value.owner == player.id) {
						steps.AddRange(stepsFromPositionWithState(state as GTMillGameSpace, kv.Key));
					}
				}

				TaskReturnType states = new TaskReturnType ();
				foreach (GTMillGameStep step in steps) {

					GTMillGameSpace newState = state.stateWithStep (step) as GTMillGameSpace;
					if (GTMillGameMillDetector.detectMillOnPositionWithStateForUser(step.to, newState, player.id))
                    {
						foreach (GTMillGameStep removeStep in removeOppenentFigureSteps(newState, player.id)) {
							states.Add(newState.stateWithStep(removeStep));
						}
                    }
					else
					{
						states.Add (newState);
					}

				}
				return states;
			});

			return task;
		}

		private Task<TaskReturnType> availableStatesThirdPhase(GTGameSpaceInterface<GTMillGameElement, GTMillPosition> state, GTPlayerInterface<GTMillGameElement, GTMillPosition> player)
		{
			Task<TaskReturnType> task = Task<TaskReturnType>.Factory.StartNew (() => {
				TaskReturnType states = new TaskReturnType ();
				return states;
			});
			return task;
		}

		private List<GTMillGameStep> removeOppenentFigureSteps(GTMillGameSpace state, int owner) {

			List<GTMillGameStep> steps = new List<GTMillGameStep> ();
			foreach (KeyValuePair<GTMillPosition, GTMillGameElement> kv in state) {
				if (kv.Value.owner != owner) {
					steps.Add (new GTMillGameStep(kv.Value, kv.Key, GTMillPosition.Nowhere()));
				}
			}

			return steps;
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
	