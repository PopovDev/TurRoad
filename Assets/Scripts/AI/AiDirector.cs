using System;
using System.Collections.Generic;
using System.Linq;
using AI.Types;
using UnityEngine;

namespace AI
{
    public class AiDirector : MonoBehaviour
    {
        [SerializeField] private PlacementManager placementManager;
        [SerializeField] private GameObject carPrefab;
        private bool _cBlock;
        private bool _canPutCar;

        public bool SpawnCar(Vector3Int position, bool abs = false, StructureModel endStructure = null)
        {
            if (!abs)
                if (_cBlock)
                    return false;
            if (!abs)
                if (!_canPutCar)
                    return false;

            var iter = 0;
            bool bn;
            while (!(bn = TrySpawnACar(position,endStructure ? endStructure : placementManager.GetRandomSpecialStructure())))
                if (iter++ > 100)
                    break;
            if (!abs) _cBlock = true;
            return bn;
        }

        public void MarkHover(Vector3Int position, GameObject mark)
        {
            var g = placementManager.GetAllHouses()
                .Any(x => (x.RoadPosition ?? new List<Vector3Int>()).Contains(position));
            mark.SetActive(_canPutCar = g);
            if (g) mark.transform.position = position;
        }

        public void FinishSpawnCar() => _cBlock = false;

        private List<Vector3> GetCarPath(Vector3Int start, Vector3Int end)
        {
            var path = placementManager.GetPathBetween(start, end, true);
            if (path.Count < 2) return new List<Vector3>();
            path.Reverse();
            var startMarker = placementManager.GetStructureAt(start).GetCarSpawnMarker(path[1]);
            var endMarker = placementManager.GetStructureAt(end).GetCarEndMarker(path[path.Count - 2]);
            var g = GetCarPath(path, startMarker.Position, endMarker.Position);

            return g == null ? new List<Vector3>() : g.ToList();
        }

        private void SpawnCar(IEnumerable<Vector3> path, Vector3 pos)
        {
            var car = Instantiate(carPrefab, pos, Quaternion.identity);
            car.GetComponent<CarAI>().SetPath(path.ToList());
        }

        private bool TrySpawnACar(Vector3Int startRoadPosition, StructureModel endStructure)
        {
            if (endStructure == null) return false;
            foreach (var g in endStructure.RoadPosition)
            {
                var carPath = GetCarPath(startRoadPosition, g);
                if (!carPath.Any()) continue;
                SpawnCar(carPath, startRoadPosition);
                return true;
            }

            return false;
        }


        private IEnumerable<Vector3> GetCarPath(IReadOnlyList<Vector3Int> path, Vector3 startPosition,
            Vector3 endPosition)
        {
            var carGraph = CreatACarGraph(path);

            if (carGraph == null) return null;
            Debug.Log(carGraph);
            return AdjacencyGraph.AStarSearch(carGraph, startPosition, endPosition);
        }

        private AdjacencyGraph CreatACarGraph(IReadOnlyList<Vector3Int> path)
        {
            var carGraph = new AdjacencyGraph();
            var tempDictionary = new Dictionary<Marker, Vector3>();
            for (var i = 0; i < path.Count; i++)
            {
                var currentPosition = path[i];
                var roadStructure = placementManager.GetStructureAt(currentPosition);
                var markersList = roadStructure.GetCarMarkers();
                var limitDistance = markersList.Count > 3;
                tempDictionary.Clear();

                foreach (var marker in markersList)
                {
                    carGraph.AddVertex(marker.Position);
                    foreach (var markerNeighbour in marker.adjacentMarkers)
                        carGraph.AddEdge(marker.Position, markerNeighbour.Position);

                    if (!marker.OpenForConnection || i + 1 >= path.Count) continue;
                    Debug.Log(i + 1);
                    Debug.Log(string.Join(" ", path));
                    Debug.Log(string.Join(" ", placementManager.StructureDictionary.Keys));
                    var nextRoadPosition = placementManager.GetStructureAt(path[i + 1]);

                    if (nextRoadPosition == null) return null;
                    if (limitDistance)
                        tempDictionary.Add(marker, nextRoadPosition.GetNearestCarMarkerTo(marker.Position));
                    else
                        carGraph.AddEdge(marker.Position, nextRoadPosition.GetNearestCarMarkerTo(marker.Position));
                }

                if (!limitDistance || tempDictionary.Count <= 2) continue;
                var distanceSortedMarkers =
                    tempDictionary.OrderBy(x => Vector3.Distance(x.Key.Position, x.Value)).ToList();
                foreach (var item in distanceSortedMarkers)
                {
                    Debug.Log(Vector3.Distance(item.Key.Position, item.Value));
                }

                for (var j = 0; j < 2; j++)
                {
                    carGraph.AddEdge(distanceSortedMarkers[j].Key.Position, distanceSortedMarkers[j].Value);
                }
            }

            return carGraph;
        }
    }
}