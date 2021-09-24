using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public int width, height;
    private Grid _placementGrid;

    private readonly Dictionary<Vector3Int, StructureModel> _temporaryRoadObjects = new Dictionary<Vector3Int, StructureModel>();
    private readonly Dictionary<Vector3Int, StructureModel> _structureDictionary = new Dictionary<Vector3Int, StructureModel>();

    private void Start()
    {
        _placementGrid = new Grid(width, height);
    }

    internal CellType[] GetNeighbourTypesFor(Vector3Int position)
    {
        return _placementGrid.GetAllAdjacentCellTypes(position.x, position.z);
    }

    internal bool CheckIfPositionInBound(Vector3Int position)
    {
        return position.x >= 0 && position.x < width && position.z >= 0 && position.z < height;
    }

    internal void PlaceObjectOnTheMap(Vector3Int position, GameObject structurePrefab, CellType type, int width = 1, int height = 1)
    {
        var structure = CreateANewStructureModel(position, structurePrefab, type);

        var structureNeedingRoad = structure.GetComponent<INeedingRoad>();
        if (structureNeedingRoad != null)
        {
            // ReSharper disable once PossibleInvalidOperationException
            structureNeedingRoad.RoadPosition = GetNearestRoad(position, width, height).Value;
            Debug.Log("My nearest road position is: " + structureNeedingRoad.RoadPosition);
        }

        for (var x = 0; x < width; x++)
        {
            for (var z = 0; z < height; z++)
            {
                var newPosition = position + new Vector3Int(x, 0, z);
                _placementGrid[newPosition.x, newPosition.z] = type;
                _structureDictionary.Add(newPosition, structure);
                DestroyNatureAt(newPosition);
            }
        }

    }

    private Vector3Int? GetNearestRoad(Vector3Int position, int w, int h)
    {
        for (var x = 0; x < w; x++)
        {
            for (var y = 0; y < h; y++)
            {
                var newPosition = position + new Vector3Int(x, 0, y);
                var roads = GetNeighboursOfTypeFor(newPosition, CellType.Road);
                if (roads.Count > 0)
                    return roads[0];
            }
        }
        return null;
    }

    private void DestroyNatureAt(Vector3Int position)
    {
        var hits = Physics.BoxCastAll(position + new Vector3(0, 0.5f, 0), new Vector3(0.5f, 0.5f, 0.5f), transform.up, Quaternion.identity, 1f, 1 << LayerMask.NameToLayer($"Nature"));
        foreach (var item in hits) Destroy(item.collider.gameObject);
    }

    internal bool CheckIfPositionIsFree(Vector3Int position)
    {
        return CheckIfPositionIsOfType(position, CellType.Empty);
    }

    private bool CheckIfPositionIsOfType(Vector3Int position, CellType type)
    {
        return _placementGrid[position.x, position.z] == type;
    }

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
        var resultPath = GridSearch.AStarSearch(_placementGrid, new Point(startPosition.x, startPosition.z), new Point(endPosition.x, endPosition.z), isAgent);
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

    internal void AddtemporaryStructuresToStructureDictionary()
    {
        foreach (var structure in _temporaryRoadObjects)
        {
            _structureDictionary.Add(structure.Key, structure.Value);
            DestroyNatureAt(structure.Key);
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

    public StructureModel GetRandomRoad()
    {
        var point = _placementGrid.GetRandomRoadPoint();
        return GetStructureAt(point);
    }

    public StructureModel GetRandomSpecialStrucutre()
    {
        var point = _placementGrid.GetRandomSpecialStructurePoint();
        return GetStructureAt(point);
    }

    public StructureModel GetRandomHouseStructure()
    {
        var point = _placementGrid.GetRandomHouseStructurePoint();
        return GetStructureAt(point);
    }

    public List<StructureModel> GetAllHouses()
    {
        List<StructureModel> returnList = new List<StructureModel>();
        var housePositions = _placementGrid.GetAllHouses();
        foreach (var point in housePositions)
        {
            returnList.Add(_structureDictionary[new Vector3Int(point.X, 0, point.Y)]);
        }
        return returnList;
    }

    internal List<StructureModel> GetAllSpecialStructures()
    {
        List<StructureModel> returnList = new List<StructureModel>();
        var housePositions = _placementGrid.GetAllSpecialStructure();
        foreach (var point in housePositions)
        {
            returnList.Add(_structureDictionary[new Vector3Int(point.X, 0, point.Y)]);
        }
        return returnList;
    }


    private StructureModel GetStructureAt(Point point)
    {
        if (point != null)
        {
            return _structureDictionary[new Vector3Int(point.X, 0, point.Y)];
        }
        return null;
    }

    public StructureModel GetStructureAt(Vector3Int position)
    {
        if (_structureDictionary.ContainsKey(position))
        {
            return _structureDictionary[position];
        }
        return null;
    }
}
