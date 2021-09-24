using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class StructureManager : MonoBehaviour
{
    public GameObject[] special;
    public GameObject[] bigStructures;
    public GameObject[] houses;
    public PlacementManager placementManager;

    public void PlaceHouse(Vector3Int position)
    {
        if (!CheckPositionBeforePlacement(position)) return;
        placementManager.PlaceObjectOnTheMap(position, houses[Random.Range(0, houses.Length)], CellType.Structure);
    }

    public void PlaceBigStructure(Vector3Int position)
    {
        const int width = 2;
        const int height = 2;
        if (!CheckBigStructure(position, width, height)) return;
        placementManager.PlaceObjectOnTheMap(position, bigStructures[Random.Range(0, bigStructures.Length)], CellType.Structure, width, height);
    }
    public void PlaceSpecial(Vector3Int position)
    {
        if (!CheckPositionBeforePlacement(position)) return;
        placementManager.PlaceObjectOnTheMap(position,
            special[Random.Range(0,special.Length)],
            CellType.SpecialStructure);
    }
    private bool CheckBigStructure(Vector3Int position, int width, int height)
    {
        var nearRoad = false;
        for (var x = 0; x < width; x++)
        {
            for (var z = 0; z < height; z++)
            {
                var newPosition = position + new Vector3Int(x, 0, z);
                
                if (DefaultCheck(newPosition)==false)
                    return false;
                if (nearRoad == false) nearRoad = RoadCheck(newPosition);
            }
        }
        return nearRoad;
    }
    private bool CheckPositionBeforePlacement(Vector3Int position) => DefaultCheck(position) && RoadCheck(position);
    private bool RoadCheck(Vector3Int position)
    {
        if (placementManager.GetNeighboursOfTypeFor(position, CellType.Road).Count > 0) return true;
        Debug.Log("Must be placed near a road");
        return false;
    }
    private bool DefaultCheck(Vector3Int position)
    {
        if (placementManager.CheckIfPositionInBound(position) == false)
        {
            Debug.Log("This position is out of bounds");
            return false;
        }
        if (placementManager.CheckIfPositionIsFree(position)) return true;
        Debug.Log("This position is not EMPTY");
        return false;
    }
}
