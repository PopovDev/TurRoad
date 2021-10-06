using System.Collections.Generic;
using System.Linq;
using AI.Types;

namespace System
{
    public static class GridSearch {
    
        public static IEnumerable<Point> AStarSearch(AGrid aGrid, Point startPosition, Point endPosition, bool isAgent = false)
        {
            var path = new List<Point>();

            var positionsCheck = new List<Point>();
            var costDictionary = new Dictionary<Point, float>();
            var priorityDictionary = new Dictionary<Point, float>();
            var parentsDictionary = new Dictionary<Point, Point>();

            positionsCheck.Add(startPosition);
            priorityDictionary.Add(startPosition, 0);
            costDictionary.Add(startPosition, 0);
            parentsDictionary.Add(startPosition, null);

            while (positionsCheck.Count > 0)
            {
                var current = GetClosestVertex(positionsCheck, priorityDictionary);
                positionsCheck.Remove(current);
                if (current.Equals(endPosition))
                {
                    path = GeneratePath(parentsDictionary, current);
                    return path;
                }

                foreach (var neighbour in aGrid.GetAdjacentCells(current))
                {
                    var newCost = costDictionary[current] + AGrid.GetCostOfEnteringCell(neighbour);
                    if (costDictionary.ContainsKey(neighbour) && !(newCost < costDictionary[neighbour])) continue;
                    costDictionary[neighbour] = newCost;
                    var priority = newCost + ManhattanDistance(endPosition, neighbour);
                    positionsCheck.Add(neighbour);
                    priorityDictionary[neighbour] = priority;
                    parentsDictionary[neighbour] = current;
                }
            }
            return path;
        }

        private static Point GetClosestVertex(IReadOnlyList<Point> list, IReadOnlyDictionary<Point, float> distanceMap)
        {
            var candidate = list[0];
            foreach (var vertex in list.Where(vertex => distanceMap[vertex] < distanceMap[candidate])) candidate = vertex;
            return candidate;
        }

        private static float ManhattanDistance(Point endPos, Point point) => 
            Math.Abs(endPos.X - point.X) + Math.Abs(endPos.Y - point.Y);

        private static List<Point> GeneratePath(IReadOnlyDictionary<Point, Point> parentMap, Point endState)
        {
            var path = new List<Point>();
            var parent = endState;
            while (parent != null && parentMap.ContainsKey(parent))
            {
                path.Add(parent);
                parent = parentMap[parent];
            }
            return path;
        }
    }
}
