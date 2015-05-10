using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTMillGameLogic
{
    public class GTMillGameMillDetector
    {

        public static bool detectMillOnPositionWithStateForUser(GTMillPosition position, GTMillGameSpace state, int owner)
        {
            List<GTMillPosition> positions = new List<GTMillPosition>();
            for (int i = 0; i < 15; i++)
            {
                if (i % 5 != 2) {
                    int[] coordinates = position.coordinates();
                    coordinates[i / 5] += -2 + (i % 5);
                    positions.Add(new GTMillPosition(coordinates[0], coordinates[1], coordinates[2]));
                }
            }

            List<GTMillPosition> availablePositions = new List<GTMillPosition>();
            foreach (GTMillPosition p in positions) {
                if (isInsideField(p)) {
                    availablePositions.Add(p);                    
                }
            }

            GTMillPosition p0;
            GTMillPosition p1;
            for (int i = 0; i < availablePositions.Count; i+=2)  {

                p0 = availablePositions[i];
                p1 = availablePositions[i+1];

                if (!isMiddlePosition(p0)
                    && !isMiddlePosition(p1)
                    && state.hasElementAt(p0)
                    && state.elementAt(p0).owner == owner
                    && state.hasElementAt(p1)
                    && state.elementAt(p1).owner == owner) {
                        return true;
                }
            }

            return false;
        }


        private static bool isInsideField(GTMillPosition p)
        {
            return p.x >= 0 && p.x < 3 && p.y >= 0 && p.y < 3 && p.z >= 0 && p.z < 3;
        }
        private static bool isMiddlePosition(GTMillPosition p)
        {
            return p.x == 1 && p.y == 1;
        }
    }
}
