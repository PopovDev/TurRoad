using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class StructureManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] special;
    [SerializeField]
    public GameObject[] houses;
    [SerializeField]
    private GameObject house;
    public RawImage housePreview;
    [SerializeField]
    private PlacementManager placementManager;
    public void SwapHouse()
    {
        var housesLength = houses.Length;
        var index = Array.IndexOf(houses, house)+1;
        if (index >= housesLength) index = 0;
        if (index < 0) index = housesLength - 1;
        house = houses[index];  
        housePreview.texture = AssetPreview.GetAssetPreview(house);
    }

    private void Start()
    {
        house = houses.FirstOrDefault();
    }

    
    public void PlaceHouse(Vector3Int position)
    {
        if (!CheckPositionBeforePlacement(position)) return;
        
        placementManager.PlaceObjectOnTheMap(position, house, CellType.Structure);
    }
    public void PlaceSpecial(Vector3Int position)
    {
        if (!CheckPositionBeforePlacement(position)) return;
        placementManager.PlaceObjectOnTheMap(position,
            special[Random.Range(0,special.Length)],
            CellType.SpecialStructure);
    }
    private bool CheckPositionBeforePlacement(Vector3Int position) => DefaultCheck(position) && RoadCheck(position);
    private bool RoadCheck(Vector3Int position) => placementManager.GetNeighboursOfTypeFor(position, CellType.Road).Count > 0;
    private bool DefaultCheck(Vector3Int position) => placementManager.CheckIfPositionInBound(position) && placementManager.IsPositionFree(position);
}
