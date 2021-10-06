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

        public void SpawnCar(Vector3Int position)
        {
            if (_cBlock) return;
            if(!_canPutCar) return;
            var iter = 0;
            while (!TrySpawnACar(position, placementManager.GetRandomSpecialStructure()))
                if (iter++ > 100)
                    break;
            _cBlock = true;
        }
        
        public void MarkHover(Vector3Int position,GameObject mark)
        {
            var g = placementManager.GetAllHouses().Any(x => x.RoadPosition.Contains(position));
            mark.SetActive(_canPutCar = g);
            if(g) mark.transform.position = position;
        }
        public void FinishSpawnCar() => _cBlock = false;

        private IEnumerable<Vector3> GetCarPath(Vector3Int start, Vector3Int end)
        {
            var path = placementManager.GetPathBetween(start, end);
            if(path.Count<2) yield break;
            Debug.Log(path.Count);
            path.Reverse();
            var startMarker = placementManager.GetStructureAt(start).GetCarSpawnMarker(path[1]);
            var endMarker = placementManager.GetStructureAt(end).GetCarEndMarker(path[path.Count - 2]);
            
            foreach (var v in GetCarPath(path, startMarker.Position, endMarker.Position))
                yield return v;

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
                var enumerable = carPath.ToList();
                if (!enumerable.Any()) continue;
                SpawnCar(enumerable, startRoadPosition);
                return true;
            }
            return false;
        }


        private List<Vector3> GetCarPath(IReadOnlyList<Vector3Int> path, Vector3 startPosition, Vector3 endPosition)
        {
            var carGraph = CreatACarGraph(path);
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
                    var nextRoadPosition = placementManager.GetStructureAt(path[i + 1]);
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