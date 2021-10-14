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
        [NonSerialized]
        public StructureModel House;
        public int carCount = 2;
        public float interval = 50;
        public float intervalP = 20;
        public bool stop = true;
    }
    
    [SerializeField] public List<Plan> plans;

    private IEnumerator Checker()
    {
        while (true)
        {
            if(gameObject == null)
                yield break;
            yield return new WaitForSecondsRealtime(0.1f);
            var toRemove = plans.Where(g => g.House == null).ToList();
            plans.RemoveAll(x => toRemove.Contains(x));
            foreach (var h in placementManager.GetAllHouses()
                .Where(h => plans
                    .All(x => x.House != h)))
            {
                plans.Add(new Plan { carCount = 0, House = h, interval = 20, intervalP = 20 });
            }

            foreach (var h in plans.Where(h => placementManager.GetAllHouses().All(x => x != h.House)))
            {
                plans.RemoveAll(x => x.House);
            }
            foreach (var j in plans.Where(x=>x.House!=null).Where(g => g.intervalP + g.interval < Time.time))
            {
                if (j.carCount <= 0) continue;
                if (j.stop)
                {
                    j.intervalP = Time.time;
                    continue;
                }
                if (!j.House.RoadPosition.Any(h =>aiDirector.SpawnCar(h, true))) continue;
                j.carCount--;
                j.intervalP = Time.time;
            }

        }
    }
    
    private void Start()
    {
        StartCoroutine(Checker());
    }

}
