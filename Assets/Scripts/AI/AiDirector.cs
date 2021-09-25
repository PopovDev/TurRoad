using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace SimpleCity.AI
{
    public class AiDirector : MonoBehaviour
    {
        [SerializeField]
        private PlacementManager placementManager;
        [SerializeField]
        private GameObject carPrefab;
        private readonly AdjacencyGraph _carGraph = new AdjacencyGraph();
        private List<Vector3> _carPath = new List<Vector3>();
        
        //ToDo: Сделать спавн для каждой
      public void SpawnACar()
        {
            foreach (var house in placementManager.GetAllHouses())
            {
                TrySpawnACar(house, placementManager.GetRandomSpecialStructure());
            }
        }

        private void TrySpawnACar(StructureModel startStructure, StructureModel endStructure)
        {
            if (startStructure == null || endStructure == null) return;
            
            var startRoadPosition = startStructure.RoadPosition;
            var endRoadPosition = endStructure.RoadPosition;

            var path = placementManager.GetPathBetween(startRoadPosition, endRoadPosition, true);
            path.Reverse();

            if (path.Count == 0 && path.Count>2) return;

            var startMarkerPosition = placementManager.GetStructureAt(startRoadPosition).GetCarSpawnMarker(path[1]);

            var endMarkerPosition = placementManager.GetStructureAt(endRoadPosition).GetCarEndMarker(path[path.Count-2]);

            _carPath = GetCarPath(path, startMarkerPosition.Position, endMarkerPosition.Position);

            if (!_carPath.Any()) return;
            
            var car = Instantiate(carPrefab, startMarkerPosition.Position, Quaternion.identity);
            car.GetComponent<CarAI>().SetPath(_carPath);
        }


        private List<Vector3> GetCarPath(IReadOnlyList<Vector3Int> path, Vector3 startPosition, Vector3 endPosition)
        {
            _carGraph.ClearGraph();
            CreatACarGraph(path);
            Debug.Log(_carGraph);
            return AdjacencyGraph.AStarSearch(_carGraph, startPosition, endPosition);
        }

        private void CreatACarGraph(IReadOnlyList<Vector3Int> path)
        {
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
                    _carGraph.AddVertex(marker.Position);
                    foreach (var markerNeighbour in marker.adjacentMarkers)
                        _carGraph.AddEdge(marker.Position, markerNeighbour.Position);

                    if (!marker.OpenForConnection || i + 1 >= path.Count) continue;
                    var nextRoadPosition = placementManager.GetStructureAt(path[i + 1]);
                    if (limitDistance)
                        tempDictionary.Add(marker, nextRoadPosition.GetNearestCarMarkerTo(marker.Position));
                    else
                        _carGraph.AddEdge(marker.Position, nextRoadPosition.GetNearestCarMarkerTo(marker.Position));
                }

                if (!limitDistance || tempDictionary.Count <= 2) continue;
                var distanceSortedMarkers = tempDictionary.OrderBy(x => Vector3.Distance(x.Key.Position, x.Value)).ToList();
                foreach (var item in distanceSortedMarkers)
                {
                    Debug.Log(Vector3.Distance(item.Key.Position, item.Value));
                }
                for (var j = 0; j < 2; j++)
                {
                    _carGraph.AddEdge(distanceSortedMarkers[j].Key.Position, distanceSortedMarkers[j].Value);
                }
            }
        }
        
        private void Update()
        {
            for (var i = 1; i < _carPath.Count; i++)
                Debug.DrawLine(_carPath[i - 1] + Vector3.up, _carPath[i] + Vector3.up, Color.magenta);
        }


    }
}

