using System;

using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;


public class MapSaver : MonoBehaviour
{ 
    public RoadManager roadManager;
    public PlacementManager placementManager;
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
        foreach (var g in  placementManager.StructureDictionary)
        {
            var type = placementManager.PlacementAGrid[g.Key.x, g.Key.z];
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
                placementManager.PlaceObjectByIndex(pos, index, cell);
        }
      
    }
       

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.K)) LoadD();

    }
}
