using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SimpleCity.AI
{
    public class AdjacencyGraph
    {
        private readonly Dictionary<Vertex, List<Vertex>> _adjacencyDictionary = new Dictionary<Vertex, List<Vertex>>();
        public void AddVertex(Vector3 position)
        {
            if(GetVertexAt(position) != null) return;
            AddVertex( new Vertex(position));
        }

        private void AddVertex(Vertex v)
        {
            if (_adjacencyDictionary.ContainsKey(v)) return;
            _adjacencyDictionary.Add(v, new List<Vertex>());
        }

        private Vertex GetVertexAt(Vector3 position)
        {
            return _adjacencyDictionary.Keys.FirstOrDefault(x => CompareVertices(position, x.Position));
        }

        private static bool CompareVertices(Vector3 position1, Vector3 position2) => Vector3.SqrMagnitude(position1 - position2) < 0.0001f;

        public void AddEdge(Vector3 position1, Vector3 position2)
        {
            if(CompareVertices(position1, position2)) return;
            
            var v1 = GetVertexAt(position1);
            var v2 = GetVertexAt(position2);
            v1 ??= new Vertex(position1);
            v2 ??= new Vertex(position2);
            AddEdgeBetween(v1, v2);
            AddEdgeBetween(v2, v1);

        }

        private void AddEdgeBetween(Vertex v1, Vertex v2)
        {
            if(v1 == v2) return;
            if (_adjacencyDictionary.ContainsKey(v1))
            {
                if(_adjacencyDictionary[v1].FirstOrDefault(x => x == v2) == null) _adjacencyDictionary[v1].Add(v2);
            }
            else
            {
                AddVertex(v1);
                _adjacencyDictionary[v1].Add(v2);
            }

        }

        private List<Vertex> GetConnectedVerticesTo(Vertex v1) => _adjacencyDictionary.ContainsKey(v1) ? _adjacencyDictionary[v1] : null;

        public List<Vertex> GetConnectedVerticesTo(Vector3 position)
        {
            var v1 = GetVertexAt(position);
            return v1 == null ? null : _adjacencyDictionary[v1];
        }

        public void ClearGraph() => _adjacencyDictionary.Clear();

        public IEnumerable<Vertex> GetVertices() => _adjacencyDictionary.Keys;

        public override string ToString()
        {
            var builder = new StringBuilder();
            foreach (var vertex in _adjacencyDictionary.Keys)
                builder.AppendLine("Vertex " + vertex + " neighbours: " +
                                   string.Join(", ", _adjacencyDictionary[vertex]));
            return builder.ToString();
        }

        public static List<Vector3> AStarSearch(AdjacencyGraph graph, Vector3 startPosition, Vector3 endPosition)
        {
            var path = new List<Vector3>();

            var start = graph.GetVertexAt(startPosition);
            var end = graph.GetVertexAt(endPosition);

            var positionsToCheck = new List<Vertex>();
            var costDictionary = new Dictionary<Vertex, float>();
            var priorityDictionary = new Dictionary<Vertex, float>();
            var parentsDictionary = new Dictionary<Vertex, Vertex>();

            positionsToCheck.Add(start);
            priorityDictionary.Add(start, 0);
            costDictionary.Add(start, 0);
            parentsDictionary.Add(start, null);

            while (positionsToCheck.Count > 0)
            {
                var current = GetClosestVertex(positionsToCheck, priorityDictionary);
                positionsToCheck.Remove(current);
                if (current.Equals(end))
                {
                    path = GeneratePath(parentsDictionary, current);
                    return path;
                }

                foreach (var neighbour in graph.GetConnectedVerticesTo(current))
                {
                    var newCost = costDictionary[current] + 1;
                    if (costDictionary.ContainsKey(neighbour) && !(newCost < costDictionary[neighbour])) continue;
                    costDictionary[neighbour] = newCost;

                    var priority = newCost + ManhattanDistance(end, neighbour);
                    positionsToCheck.Add(neighbour);
                    priorityDictionary[neighbour] = priority;

                    parentsDictionary[neighbour] = current;
                }
            }
            return path;
        }

        private static Vertex GetClosestVertex(List<Vertex> list, IReadOnlyDictionary<Vertex, float> distanceMap)
        {
            var candidate = list[0];
            foreach (var vertex in list.Where(vertex => distanceMap[vertex] < distanceMap[candidate]))
                candidate = vertex;
            return candidate;
        }

        private static float ManhattanDistance(Vertex endPos, Vertex position)
        {
            return Math.Abs(endPos.Position.x - position.Position.x) + Math.Abs(endPos.Position.z - position.Position.z);
        }

        private static List<Vector3> GeneratePath(IReadOnlyDictionary<Vertex, Vertex> parentMap, Vertex endState)
        {
            var path = new List<Vector3>();
            var parent = endState;
            while (parent != null && parentMap.ContainsKey(parent))
            {
                path.Add(parent.Position);
                parent = parentMap[parent];
            }
            path.Reverse();
            return path;
        }
    }
}

