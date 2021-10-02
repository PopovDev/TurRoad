using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class StructureManager : MonoBehaviour
{
    [SerializeField] private PlacementManager placementManager;
    [SerializeField] private GameObject[] special;
    [SerializeField] public GameObject[] houses;
    private GameObject _house;
    public RawImage housePreview;
   

    private void Start()
    {
        _house = houses.FirstOrDefault();
        UpdateIcon();
    }
    
    private void UpdateIcon()
    {
        var t = _house.GetComponent<PrefabInfo>();
        if (t != null)
        {
            housePreview.texture = t.icon;
        }
    }
    [UsedImplicitly]
    public void SwapHouse()
    {

        var housesLength = houses.Length;
        var index = Array.IndexOf(houses, _house) + 1;
        if (index >= housesLength) index = 0;
        if (index < 0) index = housesLength - 1;
        _house = houses[index];
        UpdateIcon();

    }


    public void PlaceHouse(Vector3Int position)
    {
        if (!CheckPositionBeforePlacement(position)) return;

        placementManager.PlaceObjectOnTheMap(position, _house, CellType.Structure);
    }

    public void PlaceSpecial(Vector3Int position)
    {
        if (!CheckPositionBeforePlacement(position)) return;
        placementManager.PlaceObjectOnTheMap(position,
            special[Random.Range(0, special.Length)],
            CellType.SpecialStructure);
    }

    private bool CheckPositionBeforePlacement(Vector3Int position)
    {
        return DefaultCheck(position) && RoadCheck(position);
    }

    private bool RoadCheck(Vector3Int position)
    {
        return placementManager.GetNeighboursOfTypeFor(position, CellType.Road).Count > 0;
    }

    private bool DefaultCheck(Vector3Int position)
    {
        return placementManager.CheckIfPositionInBound(position) && placementManager.IsPositionFree(position);
    }
}