using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Planner : MonoBehaviour
{
    [SerializeField] private AiDirector aiDirector;
    [SerializeField] private PlacementManager placementManager;

    [Serializable]
    public class Plan
    {
        public StructureModel house;
        public int carCount = 2;
        public float interval = 50;
        public float intervalP = 20;
    }
    
    [SerializeField] public List<Plan> plans;

    private IEnumerator Checker()
    {
        while (true)
        {
            if(gameObject == null)
                yield break;
            yield return new WaitForSecondsRealtime(0.1f);
            var toRemove = plans.Where(g => g.house == null).ToList();
            plans.RemoveAll(x => toRemove.Contains(x));
            foreach (var h in placementManager.GetAllHouses().Where(h => plans.All(x => x.house != h)))
            {
                plans.Add(new Plan { carCount = 0, house = h, interval = 20, intervalP = 20 });
            }

            foreach (var h in plans.Where(h => placementManager.GetAllHouses().All(x => x != h.house)))
            {
                plans.RemoveAll(x => x.house);
            }
            foreach (var j in plans.Where(x=>x.house!=null).Where(g => g.intervalP + g.interval < Time.time))
            {
                if (j.carCount <= 0) continue;
                if (!j.house.RoadPosition.Any(h => FindObjectOfType<AiDirector>().SpawnCar(h, true))) continue;
                j.carCount--;
                j.intervalP = Time.time;
            }

        }
    }
    
    private void Start()
    {
        StartCoroutine(Checker());
    }

    private void Update()
    {
        if (!Input.GetKeyUp(KeyCode.M)) return;
        foreach (var rn in from g in plans let c = g.house.RoadPosition.Count select g.house.RoadPosition[Random.Range(0, c)])
        {
            aiDirector.SpawnCar(rn, true);
        }
    }
}
