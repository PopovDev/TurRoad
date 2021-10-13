using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace AI
{
    public class SpecialConf : MonoBehaviour
    {
        public float timePend = 20;
        [Serializable]
        public class CarInside
        {
            public float timeIn;
            public StructureModel house;
        }

         public List<CarInside> cars;

         private IEnumerator Checker()
         {
             while (true)
             {
                 if(gameObject == null)
                     yield break;
                 yield return new WaitForSecondsRealtime(0.01f);
                 var gd = cars.Where(g => g.timeIn + timePend < Time.time)
                     .Where(g => gameObject.GetComponent<StructureModel>()
                         .RoadPosition.Any(hw => FindObjectOfType<AiDirector>()
                             .SpawnCar(hw, true, g.house))).ToList();

                 foreach (var jf in gd) cars.Remove(jf);

             }
         }

         private void OnDestroy()
         {
             StopCoroutine(Checker());
         }

         private void Start()
         {
             cars = new List<CarInside>();
             StartCoroutine(Checker());
         }
    }
}