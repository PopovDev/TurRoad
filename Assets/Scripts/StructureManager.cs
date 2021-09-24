using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class StructureManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] special;
    [SerializeField]
    private GameObject[] houses;
    [SerializeField]
    private PlacementManager placementManager;

    public void PlaceHouse(Vector3Int position)
    {
        if (!CheckPositionBeforePlacement(position)) return;
        placementManager.PlaceObjectOnTheMap(position, houses[Random.Range(0, houses.Length)], CellType.Structure);
    }
    public void PlaceSpecial(Vector3Int position)
    {
        if (!CheckPositionBeforePlacement(position)) return;
        placementManager.PlaceObjectOnTheMap(position,
            special[Random.Range(0,special.Length)],
            CellType.SpecialStructure);
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
