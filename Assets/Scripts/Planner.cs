using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AI;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Planner : MonoBehaviour
{
    [SerializeField] private AiDirector aiDirector;
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
            yield return new WaitForSecondsRealtime(0.01f);
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
