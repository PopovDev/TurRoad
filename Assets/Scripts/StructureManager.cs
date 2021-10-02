using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class StructureManager : MonoBehaviour
{
    [SerializeField] private PlacementManager placementManager;
    [SerializeField] private GameObject[] special;
    [SerializeField] public GameObject[] houses;
    public RawImage housePreview;
    public Text housePreviewText;
    public RawImage specialPreview;
    public Text specialPreviewText;
    private GameObject _house;
    private GameObject _special;

    private void Start()
    {
        _house = houses.FirstOrDefault();
        _special = houses.FirstOrDefault();
        UpdateIcon();
    }

    private void UpdateIcon()
    {
        var t = _house.GetComponent<PrefabInfo>();
        var g = _special.GetComponent<PrefabInfo>();
        if (t != null)
        {
            housePreview.texture = t.icon;
            housePreviewText.text = t.text;
        }

        if (g == null) return;
        specialPreview.texture = g.icon;
        specialPreviewText.text = g.text;
    }

    [UsedImplicitly]
    public void SwapHouse()
    {
        var index = Array.IndexOf(houses, _house) + 1;
        if (index >= houses.Length) index = 0;
        if (index < 0) index = houses.Length - 1;
        _house = houses[index];
        UpdateIcon();
    }

    [UsedImplicitly]
    public void SwapSpecial()
    {
        var index = Array.IndexOf(special, _special) + 1;
        if (index >= special.Length) index = 0;
        if (index < 0) index = special.Length - 1;
        _special = special[index];
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