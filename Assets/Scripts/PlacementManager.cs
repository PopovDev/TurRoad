using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AI.Types;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

public class PlacementManager : MonoBehaviour
{
    [SerializeField] private int width, height;
    internal AGrid PlacementAGrid;

    private readonly Dictionary<Vector3Int, StructureModel> _temporaryRoadObjects =
        new Dictionary<Vector3Int, StructureModel>();

    internal readonly Dictionary<Vector3Int, StructureModel> StructureDictionary =
        new Dictionary<Vector3Int, StructureModel>();

    private List<GameObject> _scObjs;
    public void SetObjs(IEnumerable<GameObject> a, IEnumerable<GameObject> b)
    {
        _scObjs = new List<GameObject>();
        _scObjs.AddRange(a);
        _scObjs.AddRange(b);
    }
    private void Start() => PlacementAGrid = new AGrid(width, height);

    private void NearRoadRetest()
    {
        foreach (var structure in StructureDictionary) structure.Value.RoadPosition = GetNearestRoads(structure.Key);
    }

    internal CellType[] GetNeighbourTypesFor(Vector3Int position) =>
        PlacementAGrid.GetAllAdjacentCellTypes(position.x, position.z);

    internal bool CheckIfPositionInBound(Vector3Int position) =>
        position.x >= 0 && position.x < width && position.z >= 0 && position.z < height;

    internal void PlaceObjectOnTheMap(Vector3Int position, GameObject structurePrefab, CellType type)
    {
        var structure = CreateANewStructureModel(position, structurePrefab, type);
        PlacementAGrid[position.x, position.z] = type;
        StructureDictionary.Add(position, structure);
        NearRoadRetest();
    }
    internal void PlaceObjectByIndex(Vector3Int position, int index, CellType cell) => PlaceObjectOnTheMap(position, _scObjs[index], cell);

    private IReadOnlyList<Vector3Int> GetNearestRoads(Vector3Int position)
    {
        return GetNeighboursOfTypeFor(position, CellType.Road);
    }

    public bool IsPositionFree(Vector3Int position) => TypeOfPosition(position) == CellType.Empty;
    private CellType TypeOfPosition(Vector3Int position) => PlacementAGrid[position.x, position.z];

    internal void PlaceTemporaryStructure(Vector3Int position, GameObject structurePrefab, CellType type)
    {
        PlacementAGrid[position.x, position.z] = type;
        var structure = CreateANewStructureModel(position, structurePrefab, type);
        _temporaryRoadObjects.Add(position, structure);
        NearRoadRetest();
    }

    internal List<Vector3Int> GetNeighboursOfTypeFor(Vector3Int position, CellType type)
    {
        var neighbourVertices = PlacementAGrid.GetAdjacentCellsOfType(position.x, position.z, type);
        return neighbourVertices.Select(point => new Vector3Int(point.X, 0, point.Y)).ToList();
    }

    private StructureModel CreateANewStructureModel(Vector3Int position, GameObject structurePrefab, CellType type)
    {
        var structure = new GameObject(type.ToString());

        structure.transform.SetParent(transform);
        structure.transform.localPosition = position;

        var structureModel = structure.AddComponent<StructureModel>();
        var i = Array.IndexOf(_scObjs.ToArray(), structurePrefab);
        structureModel.CreateModel(structurePrefab, i);
        return structureModel;
    }

    internal List<Vector3Int> GetPathBetween(Vector3Int startPosition, Vector3Int endPosition, bool isAgent = false)
    {
        var resultPath = GridSearch.AStarSearch(PlacementAGrid, new Point(startPosition.x, startPosition.z),
            new Point(endPosition.x, endPosition.z), isAgent);
        return resultPath.Select(point => new Vector3Int(point.X, 0, point.Y)).ToList();
    }

    internal void RemoveAllTemporaryStructures()
    {
        foreach (var structure in _temporaryRoadObjects.Values)
        {
            var position = Vector3Int.RoundToInt(structure.transform.position);
            PlacementAGrid[position.x, position.z] = CellType.Empty;
            Destroy(structure.gameObject);
        }

        _temporaryRoadObjects.Clear();
    }

    internal void AddTemporaryStructuresToStructureDictionary()
    {
        foreach (var structure in _temporaryRoadObjects)
        {
            StructureDictionary.Add(structure.Key, structure.Value);
        }

        _temporaryRoadObjects.Clear();
    }

    public void ModifyStructureModel(Vector3Int position, GameObject newModel, Quaternion rotation)
    {
        if (_temporaryRoadObjects.ContainsKey(position))
            _temporaryRoadObjects[position].SwapModel(newModel, rotation);
        else if (StructureDictionary.ContainsKey(position))
            StructureDictionary[position].SwapModel(newModel, rotation);
    }

    public StructureModel GetRandomRoad() => GetStructureAt(PlacementAGrid.GetRandomRoadPoint());

    public StructureModel GetRandomSpecialStructure() =>
        GetStructureAt(PlacementAGrid.GetRandomSpecialStructurePoint());

    public StructureModel GetRandomHouseStructure() => GetStructureAt(PlacementAGrid.GetRandomHouseStructurePoint());

    public List<StructureModel> GetAllHouses() => PlacementAGrid.GetAllHouses()
        .Select(point => StructureDictionary[new Vector3Int(point.X, 0, point.Y)]).ToList();

    internal List<StructureModel> GetAllSpecialStructures() =>
        PlacementAGrid.GetAllSpecialStructure()
            .Select(point => StructureDictionary[new Vector3Int(point.X, 0, point.Y)]).ToList();


    private StructureModel GetStructureAt(Point point) =>
        point != null ? StructureDictionary[new Vector3Int(point.X, 0, point.Y)] : null;

    public StructureModel GetStructureAt(Vector3Int position) =>
        StructureDictionary.ContainsKey(position) ? StructureDictionary[position] : null;
}