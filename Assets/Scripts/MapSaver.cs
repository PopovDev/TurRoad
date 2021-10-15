using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MapSaver : MonoBehaviour
{
    public RoadManager roadManager;
    public PlacementManager placementManager;
    public Planner planner;
    public Button saveBtn;
    public Button exitBtn;
    public static string Path { get; set; }
    public static bool LoadFromFile { get; set; }

    [Serializable]
    public class Save
    {
        public string Name { get; set; }

        [Serializable]
        public class Point3
        {
            public static Point3 ToPoint(Vector3Int f) => new Point3 { x = f.x, y = f.y, z = f.z };
            public Vector3Int ToVec() => new Vector3Int(x, y, z);
            public int x, y, z;
        }

        [Serializable]
        public class PlanData
        {
            public Point3 pos;
            public int carCount;
            public float interval;
            public bool stop;
        }

        public List<KeyValuePair<Point3, KeyValuePair<CellType, int>>> Objects { get; set; }
        public List<PlanData> PlData { get; set; }
    }

    private void SaveD()
    {
        var save = new Save
        {
            Name = "f",
            Objects = new List<KeyValuePair<Save.Point3, KeyValuePair<CellType, int>>>(),
            PlData = new List<Save.PlanData>()
        };
        foreach (var g in placementManager.StructureDictionary)
        {
            var type = placementManager.PlacementAGrid[g.Key.x, g.Key.z];
            var index = g.Value.ObjIndex;
            Debug.Log(index);
            save.Objects.Add(
                new KeyValuePair<Save.Point3, KeyValuePair<CellType, int>>(
                    Save.Point3.ToPoint(g.Key),
                    new KeyValuePair<CellType, int>(type, index)));
            foreach (var ge in planner.plans)
            {
                save.PlData.Add(new Save.PlanData
                {
                    carCount = ge.carCount,
                    interval = ge.interval,
                    stop = ge.stop,
                    pos = Save.Point3.ToPoint(ge.House.Pos)
                });
            }
        }

        File.WriteAllText(Path, JsonConvert.SerializeObject(save));
    }

    private async Task LoadD()
    {
        var save = new Save();
        await Task.Run(() =>
        {
            save = JsonConvert.DeserializeObject<Save>(File.ReadAllText(Path));
            Debug.Log(save.Name);
        });
        foreach (var obj in save.Objects)
        {
            var pos = obj.Key.ToVec();
            var index = obj.Value.Value;
            var cell = obj.Value.Key;
            Debug.Log(index);
            if (index == -1)
            {
                await roadManager.PlaceRoad(pos, false);
                roadManager.FinishPlacingRoad();
            }
            else
                placementManager.PlaceObjectByIndex(pos, index, cell);
        }

        await Task.Delay(100);
        foreach (var g in save.PlData)
        {
            foreach (var gg in planner.plans.Where(gg => gg.House.Pos == g.pos.ToVec()))
            {
                gg.interval = g.interval;
                gg.stop = g.stop;
                gg.carCount = g.carCount;
            }
        }
    }

    private async void Start()
    {
        exitBtn.onClick.AddListener(() => SceneManager.LoadScene(0, LoadSceneMode.Single));
        saveBtn.onClick.AddListener(SaveD);
        if (LoadFromFile) await LoadD();
    }
}