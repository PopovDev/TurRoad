using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    public PlacementManager placementManager;


    public List<Vector3Int> temporaryPlacementPositions = new List<Vector3Int>();
    public List<Vector3Int> roadPositionsToRecheck = new List<Vector3Int>();
    private Vector3Int _startPosition;
    private bool _placementMode;
    private RoadFixer _roadFixer;

    private void Start()
    {
        _roadFixer = GetComponent<RoadFixer>();
    }

    public async Task PlaceRoad(Vector3Int pos, bool canBreak )
    {
        await Task.Yield();
        if (canBreak)
            if (Input.GetKey(KeyCode.LeftControl))
            {
                var f = placementManager.PlacementAGrid[pos.x, pos.z];
                if (f == CellType.Road) placementManager.RemoveObject(pos);
                FixRoadPrefabs();
                return;
            }

        if (placementManager.CheckIfPositionInBound(pos) == false)
            return;
        if (placementManager.IsPositionFree(pos) == false)
            return;
        if (_placementMode == false)
        {
            temporaryPlacementPositions.Clear();
            roadPositionsToRecheck.Clear();

            _placementMode = true;
            _startPosition = pos;

            temporaryPlacementPositions.Add(pos);
            placementManager.PlaceTemporaryStructure(pos, _roadFixer.deadEnd, CellType.Road);
        }
        else
        {
            placementManager.RemoveAllTemporaryStructures();
            temporaryPlacementPositions.Clear();

            foreach (var positionsToFix in roadPositionsToRecheck)
            {
                _roadFixer.FixRoadAtPosition(placementManager, positionsToFix);
            }

            roadPositionsToRecheck.Clear();

            temporaryPlacementPositions = placementManager.GetPathBetween(_startPosition, pos);

            foreach (var temporaryPosition in temporaryPlacementPositions)
            {
                if (placementManager.IsPositionFree(temporaryPosition) == false)
                {
                    roadPositionsToRecheck.Add(temporaryPosition);
                    continue;
                }

                placementManager.PlaceTemporaryStructure(temporaryPosition, _roadFixer.deadEnd, CellType.Road);
            }
        }

        FixRoadPrefabs();
    }

    private void FixRoadPrefabs()
    {
        foreach (var temporaryPosition in temporaryPlacementPositions)
        {
            _roadFixer.FixRoadAtPosition(placementManager, temporaryPosition);
            var neighbours = placementManager.GetNeighboursOfTypeFor(temporaryPosition, CellType.Road);
            foreach (var roadPosition in neighbours.Where(x =>
                roadPositionsToRecheck.Contains(x) == false))
            {
                roadPositionsToRecheck.Add(roadPosition);
            }
        }

        foreach (var positionToFix in roadPositionsToRecheck)
        {
            _roadFixer.FixRoadAtPosition(placementManager, positionToFix);
        }
    }

    public void FinishPlacingRoad()
    {
        _placementMode = false;
        placementManager.AddTemporaryStructuresToStructureDictionary();
        temporaryPlacementPositions.Clear();
        _startPosition = Vector3Int.zero;
    }

    public Task PlaceRoad(Vector3Int obj) => PlaceRoad(obj,true);
}