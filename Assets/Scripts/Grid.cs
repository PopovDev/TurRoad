using System;
using System.Collections.Generic;

public enum CellType
{
    Empty,
    Road,
    Structure,
    SpecialStructure,
    None
}

public class Grid
{
    private readonly CellType[,] _grid;
    private int Width { get; }
    private int Height { get; }

    private readonly List<Point> _roadList = new List<Point>();
    private readonly List<Point> _specialStructure = new List<Point>();
    private readonly List<Point> _houseStructure = new List<Point>();

    public Grid(int width, int height)
    {
        Width = width;
        Height = height;
        _grid = new CellType[width, height];
    }

    // Adding index operator to our Grid class so that we can use grid[][] to access specific cell from our grid. 
    public CellType this[int i, int j]
    {
        get => _grid[i, j];
        set
        {
            switch (value)
            {
                case CellType.Road:
                    _roadList.Add(new Point(i, j));
                    break;
                case CellType.SpecialStructure:
                    _specialStructure.Add(new Point(i, j));
                    break;
                case CellType.Structure:
                    _houseStructure.Add(new Point(i, j));
                    break;
                case CellType.Empty:
                    break;
                case CellType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }

            _grid[i, j] = value;
        }
    }

    private static bool IsCellWalkable(CellType cellType, bool aiAgent = false)
    {
        if (aiAgent)
            return cellType == CellType.Road;
        return cellType == CellType.Empty || cellType == CellType.Road;
    }

    public Point GetRandomRoadPoint() =>
        _roadList.Count == 0 ? null : _roadList[UnityEngine.Random.Range(0, _roadList.Count)];

    public Point GetRandomSpecialStructurePoint() => _specialStructure.Count == 0
        ? null
        : _specialStructure[UnityEngine.Random.Range(0, _specialStructure.Count)];

    public Point GetRandomHouseStructurePoint() => _houseStructure.Count == 0
        ? null
        : _houseStructure[UnityEngine.Random.Range(0, _houseStructure.Count)];

    public List<Point> GetAllHouses() => _houseStructure;

    internal List<Point> GetAllSpecialStructure() => _specialStructure;

    public List<Point> GetAdjacentCells(Point cell, bool isAgent)
    {
        return GetWalkableAdjacentCells((int)cell.X, (int)cell.Y, isAgent);
    }

    public static float GetCostOfEnteringCell(Point cell) => 1;

    private List<Point> GetAllAdjacentCells(int x, int y)
    {
        var adjacentCells = new List<Point>();
        if (x > 0) adjacentCells.Add(new Point(x - 1, y));
        if (x < Width - 1) adjacentCells.Add(new Point(x + 1, y));
        if (y > 0) adjacentCells.Add(new Point(x, y - 1));
        if (y < Height - 1) adjacentCells.Add(new Point(x, y + 1));
        return adjacentCells;
    }

    private List<Point> GetWalkableAdjacentCells(int x, int y, bool isAgent)
    {
        var adjacentCells = GetAllAdjacentCells(x, y);
        for (var i = adjacentCells.Count - 1; i >= 0; i--)
            if (IsCellWalkable(_grid[adjacentCells[i].X, adjacentCells[i].Y], isAgent) == false)
                adjacentCells.RemoveAt(i);
        return adjacentCells;
    }

    public List<Point> GetAdjacentCellsOfType(int x, int y, CellType type)
    {
        var adjacentCells = GetAllAdjacentCells(x, y);
        for (var i = adjacentCells.Count - 1; i >= 0; i--)
            if (_grid[adjacentCells[i].X, adjacentCells[i].Y] != type)
                adjacentCells.RemoveAt(i);
        return adjacentCells;
    }

    /// <summary>
    /// Returns array [Left neighbour, Top neighbour, Right neighbour, Down neighbour]
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public CellType[] GetAllAdjacentCellTypes(int x, int y)
    {
        CellType[] neighbours = { CellType.None, CellType.None, CellType.None, CellType.None };
        if (x > 0) neighbours[0] = _grid[x - 1, y];
        if (x < Width - 1) neighbours[2] = _grid[x + 1, y];
        if (y > 0) neighbours[3] = _grid[x, y - 1];
        if (y < Height - 1) neighbours[1] = _grid[x, y + 1];
        return neighbours;
    }
}