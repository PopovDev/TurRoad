using System;
using System.Linq;
using UnityEngine;

public class StructureManager : MonoBehaviour
{
    public StructurePrefabWeighted[] housesPrefabs;
    public StructurePrefabWeighted[] specialPrefabs, bigStructuresPrefabs;
    public PlacementManager placementManager;

    private float[] _houseWeights, _specialWeights, _bigStructureWeights;

    private void Start()
    {
        _houseWeights = housesPrefabs.Select(prefabStats => prefabStats.weight).ToArray();
        _specialWeights = specialPrefabs.Select(prefabStats => prefabStats.weight).ToArray();
        _bigStructureWeights = bigStructuresPrefabs.Select(prefabStats => prefabStats.weight).ToArray();
    }

    public void PlaceHouse(Vector3Int position)
    {
        if (!CheckPositionBeforePlacement(position)) return;
        var randomIndex = GetRandomWeightedIndex(_houseWeights);
        placementManager.PlaceObjectOnTheMap(position, housesPrefabs[randomIndex].prefab, CellType.Structure);
    }

    internal void PlaceBigStructure(Vector3Int position)
    {
        const int width = 2;
        const int height = 2;
        if (!CheckBigStructure(position, width, height)) return;
        var randomIndex = GetRandomWeightedIndex(_bigStructureWeights);
        placementManager.PlaceObjectOnTheMap(position, bigStructuresPrefabs[randomIndex].prefab, CellType.Structure, width, height);
    }

    private bool CheckBigStructure(Vector3Int position, int width, int height)
    {
        var nearRoad = false;
        for (var x = 0; x < width; x++)
        {
            for (var z = 0; z < height; z++)
            {
                var newPosition = position + new Vector3Int(x, 0, z);
                
                if (DefaultCheck(newPosition)==false)
                {
                    return false;
                }
                if (nearRoad == false)
                {
                    nearRoad = RoadCheck(newPosition);
                }
            }
        }
        return nearRoad;
    }
    public void PlaceSpecial(Vector3Int position)
    {
        if (!CheckPositionBeforePlacement(position)) return;
        var randomIndex = GetRandomWeightedIndex(_specialWeights);
        placementManager.PlaceObjectOnTheMap(position, specialPrefabs[randomIndex].prefab, CellType.SpecialStructure);
    }
    public static int GetRandomWeightedIndex(float[] weights)
    {
        var sum = weights.Sum();
        var randomValue = UnityEngine.Random.Range(0, sum);
        float tempSum = 0;
        for (var i = 0; i < weights.Length; i++)
        {
            if(randomValue >= tempSum && randomValue < tempSum + weights[i])
                return i;
            tempSum += weights[i];
        }
        return 0;
    }
    private bool CheckPositionBeforePlacement(Vector3Int position) => DefaultCheck(position) && RoadCheck(position);
    private bool RoadCheck(Vector3Int position)
    {
        if (placementManager.GetNeighboursOfTypeFor(position, CellType.Road).Count > 0) return true;
        Debug.Log("Must be placed near a road");
        return false;
    }
    private bool DefaultCheck(Vector3Int position)
    {
        if (placementManager.CheckIfPositionInBound(position) == false)
        {
            Debug.Log("This position is out of bounds");
            return false;
        }
        if (placementManager.CheckIfPositionIsFree(position)) return true;
        Debug.Log("This position is not EMPTY");
        return false;
    }
}

[Serializable]
public struct StructurePrefabWeighted
{
    public GameObject prefab;
    [Range(0,1)]
    public float weight;
}
