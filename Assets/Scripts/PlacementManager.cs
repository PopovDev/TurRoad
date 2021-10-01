using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AI.Types;
using UnityEngine;

public class PlacementManager : MonoBehaviour 
{
    [SerializeField]
    private int width, height;
    private Grid _placementGrid;
    private readonly Dictionary<Vector3Int, StructureModel> _temporaryRoadObjects = new Dictionary<Vector3Int, StructureModel>();
    private readonly Dictionary<Vector3Int, StructureModel> _structureDictionary = new Dictionary<Vector3Int, StructureModel>();
    private void Start() => _placementGrid = new Grid(width, height);
    internal CellType[] GetNeighbourTypesFor(Vector3Int position) => _placementGrid.GetAllAdjacentCellTypes(position.x, position.z);

    internal bool CheckIfPositionInBound(Vector3Int position) => position.x >= 0 && position.x < width && position.z >= 0 && position.z < height;

    internal void PlaceObjectOnTheMap(Vector3Int position, GameObject structurePrefab, CellType type)
    {
        var structure = CreateANewStructureModel(position, structurePrefab, type);

        var structureNeedingRoad = structure.GetComponent<INeedingRoad>();
        if (structureNeedingRoad != null)
        {
            structureNeedingRoad.RoadPosition = GetNearestRoad(position)?? new Vector3Int(0,0,0);
            Debug.Log("My nearest road position is: " + structureNeedingRoad.RoadPosition);
        }
        _placementGrid[position.x, position.z] = type;
        _structureDictionary.Add(position, structure);
    }

    private Vector3Int? GetNearestRoad(Vector3Int position)
    {
        var roads = GetNeighboursOfTypeFor(position, CellType.Road);
        if (roads.Count > 0) return roads[0];

        return null;
    }
    
    public bool IsPositionFree(Vector3Int position) => TypeOfPosition(position)==CellType.Empty;
    private CellType TypeOfPosition(Vector3Int position) => _placementGrid[position.x, position.z];

    internal void PlaceTemporaryStructure(Vector3Int position, GameObject structurePrefab, CellType type)
    {
        _placementGrid[position.x, position.z] = type;
        var structure = CreateANewStructureModel(position, structurePrefab, type);
        _temporaryRoadObjects.Add(position, structure);
    }

    internal List<Vector3Int> GetNeighboursOfTypeFor(Vector3Int position, CellType type)
    {
        var neighbourVertices = _placementGrid.GetAdjacentCellsOfType(position.x, position.z, type);
        return neighbourVertices.Select(point => new Vector3Int(point.X, 0, point.Y)).ToList();
    }

    private StructureModel CreateANewStructureModel(Vector3Int position, GameObject structurePrefab, CellType type)
    {
        var structure = new GameObject(type.ToString());
        
        structure.transform.SetParent(transform);
        structure.transform.localPosition = position;

        var structureModel = structure.AddComponent<StructureModel>();
        structureModel.CreateModel(structurePrefab);
        return structureModel;
    }

    internal List<Vector3Int> GetPathBetween(Vector3Int startPosition, Vector3Int endPosition, bool isAgent = false)
    {
        var resultPath = GridSearch.AStarSearch(_placementGrid, new Point(startPosition.x, startPosition.z),
            new Point(endPosition.x, endPosition.z), isAgent);
        return resultPath.Select(point => new Vector3Int(point.X, 0, point.Y)).ToList();
    }

    internal void RemoveAllTemporaryStructures()
    {
        foreach (var structure in _temporaryRoadObjects.Values)
        {
            var position = Vector3Int.RoundToInt(structure.transform.position);
            _placementGrid[position.x, position.z] = CellType.Empty;
            Destroy(structure.gameObject);
        }
        _temporaryRoadObjects.Clear();
    }

    internal void AddTemporaryStructuresToStructureDictionary()
    {
        foreach (var structure in _temporaryRoadObjects)
        {
            _structureDictionary.Add(structure.Key, structure.Value);
        }

        _temporaryRoadObjects.Clear();
    }

    public void ModifyStructureModel(Vector3Int position, GameObject newModel, Quaternion rotation)
    {
        if (_temporaryRoadObjects.ContainsKey(position))
            _temporaryRoadObjects[position].SwapModel(newModel, rotation);
        else if (_structureDictionary.ContainsKey(position))
            _structureDictionary[position].SwapModel(newModel, rotation);
    }

    public StructureModel GetRandomRoad() => GetStructureAt(_placementGrid.GetRandomRoadPoint());

    public StructureModel GetRandomSpecialStructure() => GetStructureAt(_placementGrid.GetRandomSpecialStructurePoint());

    public StructureModel GetRandomHouseStructure() => GetStructureAt(_placementGrid.GetRandomHouseStructurePoint());

    public List<StructureModel> GetAllHouses() => _placementGrid.GetAllHouses().Select(point => _structureDictionary[new Vector3Int(point.X, 0, point.Y)]).ToList();

    internal List<StructureModel> GetAllSpecialStructures() =>
        _placementGrid.GetAllSpecialStructure()
            .Select(point => _structureDictionary[new Vector3Int(point.X, 0, point.Y)]).ToList();


    private StructureModel GetStructureAt(Point point) =>
        point != null ? _structureDictionary[new Vector3Int(point.X, 0, point.Y)] : null;

    public StructureModel GetStructureAt(Vector3Int position) => 
        _structureDictionary.ContainsKey(position) ? _structureDictionary[position] : null;
}