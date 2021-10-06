using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class RoadFixer : MonoBehaviour
{
    #region roadTemp

    public GameObject deadEnd;
    public GameObject roadStraight;
    public GameObject corner;
    public GameObject threeWay;
    public GameObject fourWay;
    #endregion

    public void FixRoadAtPosition(PlacementManager placementManager, Vector3Int temporaryPosition)
    {
        //[right, up, left, down]
        var result = placementManager.GetNeighbourTypesFor(temporaryPosition);
        var roadCount = 0;
        roadCount = result.Count(x => x == CellType.Road);
        switch (roadCount)
        {
            case 0:
            case 1:
                CreateDeadEnd(placementManager, result, temporaryPosition);
                break;
            case 2 when CreateStraightRoad(placementManager, result, temporaryPosition):
                return;
            case 2:
                CreateCorner(placementManager, result, temporaryPosition);
                break;
            case 3:
                Create3Way(placementManager, result, temporaryPosition);
                break;
            default:
                Create4Way(placementManager, result, temporaryPosition);
                break;
        }
    }

    private void Create4Way(PlacementManager placementManager, CellType[] result, Vector3Int temporaryPosition)
    {
        placementManager.ModifyStructureModel(temporaryPosition, fourWay, Quaternion.identity);
    }

    //[left, up, right, down]
    private void Create3Way(PlacementManager placementManager, IReadOnlyList<CellType> result,
        Vector3Int temporaryPosition)
    {
        if (result[1] == CellType.Road && result[2] == CellType.Road && result[3] == CellType.Road)
        {
            placementManager.ModifyStructureModel(temporaryPosition, threeWay, Quaternion.identity);
        }
        else if (result[2] == CellType.Road && result[3] == CellType.Road && result[0] == CellType.Road)
        {
            placementManager.ModifyStructureModel(temporaryPosition, threeWay, Quaternion.Euler(0, 90, 0));
        }
        else if (result[3] == CellType.Road && result[0] == CellType.Road && result[1] == CellType.Road)
        {
            placementManager.ModifyStructureModel(temporaryPosition, threeWay, Quaternion.Euler(0, 180, 0));
        }
        else if (result[0] == CellType.Road && result[1] == CellType.Road && result[2] == CellType.Road)
        {
            placementManager.ModifyStructureModel(temporaryPosition, threeWay, Quaternion.Euler(0, 270, 0));
        }
    }

    //[left, up, right, down]
    private void CreateCorner(PlacementManager placementManager, IReadOnlyList<CellType> result,
        Vector3Int temporaryPosition)
    {
        if (result[1] == CellType.Road && result[2] == CellType.Road)
        {
            placementManager.ModifyStructureModel(temporaryPosition, corner, Quaternion.Euler(0, 90, 0));
        }
        else if (result[2] == CellType.Road && result[3] == CellType.Road)
        {
            placementManager.ModifyStructureModel(temporaryPosition, corner, Quaternion.Euler(0, 180, 0));
        }
        else if (result[3] == CellType.Road && result[0] == CellType.Road)
        {
            placementManager.ModifyStructureModel(temporaryPosition, corner, Quaternion.Euler(0, 270, 0));
        }
        else if (result[0] == CellType.Road && result[1] == CellType.Road)
        {
            placementManager.ModifyStructureModel(temporaryPosition, corner, Quaternion.identity);
        }
    }

    //[left, up, right, down]
    private bool CreateStraightRoad(PlacementManager placementManager, IReadOnlyList<CellType> result,
        Vector3Int temporaryPosition)
    {
        if (result[0] == CellType.Road && result[2] == CellType.Road)
        {
            placementManager.ModifyStructureModel(temporaryPosition, roadStraight, Quaternion.identity);
            return true;
        }
        else if (result[1] == CellType.Road && result[3] == CellType.Road)
        {
            placementManager.ModifyStructureModel(temporaryPosition, roadStraight, Quaternion.Euler(0, 90, 0));
            return true;
        }

        return false;
    }

    //[left, up, right, down]
    private void CreateDeadEnd(PlacementManager placementManager, IReadOnlyList<CellType> result,
        Vector3Int temporaryPosition)
    {
        if (result[1] == CellType.Road)
            placementManager.ModifyStructureModel(temporaryPosition, deadEnd, Quaternion.Euler(0, 270, 0));
        else if (result[2] == CellType.Road)
            placementManager.ModifyStructureModel(temporaryPosition, deadEnd, Quaternion.identity);
        else if (result[3] == CellType.Road)
            placementManager.ModifyStructureModel(temporaryPosition, deadEnd, Quaternion.Euler(0, 90, 0));
        else if (result[0] == CellType.Road)
            placementManager.ModifyStructureModel(temporaryPosition, deadEnd, Quaternion.Euler(0, 180, 0));
    }
}