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
    private AGrid _placementAGrid;

    private readonly Dictionary<Vector3Int, StructureModel> _temporaryRoadObjects =
        new Dictionary<Vector3Int, StructureModel>();

    private readonly Dictionary<Vector3Int, StructureModel> _structureDictionary =
        new Dictionary<Vector3Int, StructureModel>();

    private List<GameObject> _scObjs;
    public RoadManager roadManager;
    public RoadFixer roadFixer;

    public void SetObjs(IEnumerable<GameObject> a, IEnumerable<GameObject> b)
    {
        _scObjs = new List<GameObject>();
        _scObjs.AddRange(a);
        _scObjs.AddRange(b);
    }

    private void Start() => _placementAGrid = new AGrid(width, height);

    [Serializable]
    private class Save
    {
        public string Name { get; set; }

        [Serializable]
        public class Point3
        {
            public static Point3 ToPoint(Vector3Int f) => new Point3 { x = f.x, y = f.y, z = f.z };
            public Vector3Int ToVec() => new Vector3Int(x, y, z);
            public int x, y, z;
        }

        public List<KeyValuePair<Point3, KeyValuePair<CellType, int>>> Objects { get; set; }
    }

    private void SavD()
    {
        var save = new Save
        {
            Name = "f",
            Objects = new List<KeyValuePair<Save.Point3, KeyValuePair<CellType, int>>>()
        };
        foreach (var g in _structureDictionary)
        {
            var type = _placementAGrid[g.Key.x, g.Key.z];
            var index = g.Value.ObjIndex;
            save.Objects.Add(
                new KeyValuePair<Save.Point3, KeyValuePair<CellType, int>>(
                    Save.Point3.ToPoint(g.Key),
                    new KeyValuePair<CellType, int>(type, index)));
        }

        File.WriteAllText("Assets/data.json", JsonConvert.SerializeObject(save));
    }

    private void LoadD()
    {
        var save = JsonConvert.DeserializeObject<Save>(File.ReadAllText("Assets/data.json"));
        Debug.Log(save.Name);
        foreach (var obj in save.Objects)
        {
            var pos = obj.Key.ToVec();
            var index = obj.Value.Value;
            var cell = obj.Value.Key;
            if (index == -1)
            {
                roadManager.PlaceRoad(pos);
                roadManager.FinishPlacingRoad();
            }
            else
                PlaceObjectOnTheMap(pos, _scObjs[index], cell);

            
        }
    }

    private void Update()
    {
        if (!Input.GetMouseButtonUp(1)) return;
        LoadD();

    }

    private void NearRoadRetest()
    {
        foreach (var structure in _structureDictionary) structure.Value.RoadPosition = GetNearestRoads(structure.Key);
    }

    internal CellType[] GetNeighbourTypesFor(Vector3Int position) =>
        _placementAGrid.GetAllAdjacentCellTypes(position.x, position.z);

    internal bool CheckIfPositionInBound(Vector3Int position) =>
        position.x >= 0 && position.x < width && position.z >= 0 && position.z < height;

    internal void PlaceObjectOnTheMap(Vector3Int position, GameObject structurePrefab, CellType type)
    {
        var structure = CreateANewStructureModel(position, structurePrefab, type);
        _placementAGrid[position.x, position.z] = type;
        _structureDictionary.Add(position, structure);
        NearRoadRetest();
    }

    private IReadOnlyList<Vector3Int> GetNearestRoads(Vector3Int position)
    {
        return GetNeighboursOfTypeFor(position, CellType.Road);
    }

    public bool IsPositionFree(Vector3Int position) => TypeOfPosition(position) == CellType.Empty;
    private CellType TypeOfPosition(Vector3Int position) => _placementAGrid[position.x, position.z];

    internal void PlaceTemporaryStructure(Vector3Int position, GameObject structurePrefab, CellType type)
    {
        _placementAGrid[position.x, position.z] = type;
        var structure = CreateANewStructureModel(position, structurePrefab, type);
        _temporaryRoadObjects.Add(position, structure);
        NearRoadRetest();
    }

    internal List<Vector3Int> GetNeighboursOfTypeFor(Vector3Int position, CellType type)
    {
        var neighbourVertices = _placementAGrid.GetAdjacentCellsOfType(position.x, position.z, type);
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
        var resultPath = GridSearch.AStarSearch(_placementAGrid, new Point(startPosition.x, startPosition.z),
            new Point(endPosition.x, endPosition.z), isAgent);
        return resultPath.Select(point => new Vector3Int(point.X, 0, point.Y)).ToList();
    }

    internal void RemoveAllTemporaryStructures()
    {
        foreach (var structure in _temporaryRoadObjects.Values)
        {
            var position = Vector3Int.RoundToInt(structure.transform.position);
            _placementAGrid[position.x, position.z] = CellType.Empty;
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

    public StructureModel GetRandomRoad() => GetStructureAt(_placementAGrid.GetRandomRoadPoint());

    public StructureModel GetRandomSpecialStructure() =>
        GetStructureAt(_placementAGrid.GetRandomSpecialStructurePoint());

    public StructureModel GetRandomHouseStructure() => GetStructureAt(_placementAGrid.GetRandomHouseStructurePoint());

    public List<StructureModel> GetAllHouses() => _placementAGrid.GetAllHouses()
        .Select(point => _structureDictionary[new Vector3Int(point.X, 0, point.Y)]).ToList();

    internal List<StructureModel> GetAllSpecialStructures() =>
        _placementAGrid.GetAllSpecialStructure()
            .Select(point => _structureDictionary[new Vector3Int(point.X, 0, point.Y)]).ToList();


    private StructureModel GetStructureAt(Point point) =>
        point != null ? _structureDictionary[new Vector3Int(point.X, 0, point.Y)] : null;

    public StructureModel GetStructureAt(Vector3Int position) =>
        _structureDictionary.ContainsKey(position) ? _structureDictionary[position] : null;
}