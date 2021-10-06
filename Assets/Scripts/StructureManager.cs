using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class StructureManager : MonoBehaviour
{
    [SerializeField] private PlacementManager placementManager;
    [SerializeField] private GameObject[] special;
    [SerializeField] public GameObject[] houses;
    public RawImage housePreview;
    public Text housePreviewText;
    public RawImage specialPreview;
    public Text specialPreviewText;
    private bool _canPlace;
    private GameObject _house;
    private bool _placed;
    private GameObject _special;

    private void Start()
    {
        _house = houses.FirstOrDefault();
        _special = special.FirstOrDefault();
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
        if (_placed) return;
        if (_canPlace)
            placementManager.PlaceObjectOnTheMap(position, _house, CellType.Structure);
        _placed = true;
    }

    public void PlaceHover(Vector3Int pos, GameObject mark)
    {
        mark.SetActive(_canPlace = DefaultCheck(pos) && RoadCheck(pos));
        if (_canPlace) mark.transform.position = pos;
    }

    public void PlaceSpecial(Vector3Int position)
    {
        if (_placed) return;
        if (_canPlace)
            placementManager.PlaceObjectOnTheMap(position, _special, CellType.SpecialStructure);
        _placed = true;
    }

    private bool RoadCheck(Vector3Int position)
    {
        return placementManager.GetNeighboursOfTypeFor(position, CellType.Road).Count > 0;
    }

    private bool DefaultCheck(Vector3Int position)
    {
        return placementManager.CheckIfPositionInBound(position) && placementManager.IsPositionFree(position);
    }

    public void FinishPlace() => _placed = false;
}