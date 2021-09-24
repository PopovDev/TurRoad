using System;
using System.Collections.Generic;

public class Point
{
    public int X { get; }
    public int Y { get; }

    public Point(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is Point point)) return false;
        return X == point.X && Y == point.Y;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 6949;
            hash = hash * 7907 + X.GetHashCode();
            hash = hash * 7907 + Y.GetHashCode();
            return hash;
        }
    }

    public override string ToString()
    {
        return "P(" + X + ", " + Y + ")";
    }
}

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
    private CellType[,] _grid;
    private int _width;
    public int Width { get { return _width; } }
    private int _height;
    public int Height { get { return _height; } }

    private List<Point> _roadList = new List<Point>();
    private List<Point> _specialStructure = new List<Point>();
    private List<Point> _houseStructure = new List<Point>();

    public Grid(int width, int height)
    {
        _width = width;
        _height = height;
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

    public static bool IsCellWakable(CellType cellType, bool aiAgent = false)
    {
        if (aiAgent)
        {
            return cellType == CellType.Road;
        }
        return cellType == CellType.Empty || cellType == CellType.Road;
    }

    public Point GetRandomRoadPoint()
    {
        return _roadList.Count == 0 ? null : _roadList[UnityEngine.Random.Range(0, _roadList.Count)];
    }

    public Point GetRandomSpecialStructurePoint()
    {
        return _specialStructure.Count == 0 ? null : _specialStructure[UnityEngine.Random.Range(0, _specialStructure.Count)];
    }

    public Point GetRandomHouseStructurePoint()
    {
        return _houseStructure.Count == 0 ? null : _houseStructure[UnityEngine.Random.Range(0, _houseStructure.Count)];
    }

    public List<Point> GetAllHouses() => _houseStructure;

    internal List<Point> GetAllSpecialStructure() => _specialStructure;

    public List<Point> GetAdjacentCells(Point cell, bool isAgent)
    {
        return GetWakableAdjacentCells((int)cell.X, (int)cell.Y, isAgent);
    }

    public float GetCostOfEnteringCell(Point cell)
    {
        return 1;
    }

    private List<Point> GetAllAdjacentCells(int x, int y)
    {
        var adjacentCells = new List<Point>();
        if (x > 0)
        {
            adjacentCells.Add(new Point(x - 1, y));
        }
        if (x < _width - 1)
        {
            adjacentCells.Add(new Point(x + 1, y));
        }
        if (y > 0)
        {
            adjacentCells.Add(new Point(x, y - 1));
        }
        if (y < _height - 1)
        {
            adjacentCells.Add(new Point(x, y + 1));
        }
        return adjacentCells;
    }

    public List<Point> GetWakableAdjacentCells(int x, int y, bool isAgent)
    {
        List<Point> adjacentCells = GetAllAdjacentCells(x, y);
        for (int i = adjacentCells.Count - 1; i >= 0; i--)
        {
            if (IsCellWakable(_grid[adjacentCells[i].X, adjacentCells[i].Y], isAgent) == false)
            {
                adjacentCells.RemoveAt(i);
            }
        }
        return adjacentCells;
    }

    public List<Point> GetAdjacentCellsOfType(int x, int y, CellType type)
    {
        List<Point> adjacentCells = GetAllAdjacentCells(x, y);
        for (int i = adjacentCells.Count - 1; i >= 0; i--)
        {
            if (_grid[adjacentCells[i].X, adjacentCells[i].Y] != type)
            {
                adjacentCells.RemoveAt(i);
            }
        }
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
        if (x > 0)
        {
            neighbours[0] = _grid[x - 1, y];
        }
        if (x < _width - 1)
        {
            neighbours[2] = _grid[x + 1, y];
        }
        if (y > 0)
        {
            neighbours[3] = _grid[x, y - 1];
        }
        if (y < _height - 1)
        {
            neighbours[1] = _grid[x, y + 1];
        }
        return neighbours;
    }
}
