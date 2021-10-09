using System;
using System.Collections.Generic;
using UnityEngine;

namespace AI.TrafficLights
{
    public class LightController : MonoBehaviour
    {
        [SerializeField] private TrafficGate p1;
        [SerializeField] private TrafficGate p2;
        [Space]
        [SerializeField] public LightState state;
        [Space]
        [SerializeField] private List<TrafficLight> oneGroup;
        [SerializeField] private  List<TrafficLight> twoGroup;

        public enum LightState
        {
            N1,
            YellowN1,
            N2,
            YellowN2,
        }

        private void Start() => Changed();

        internal void Changed()
        {
            switch (state)
            {
                case LightState.N1:
                    p1.canRun = true;
                    p2.canRun = false;
                    oneGroup.ForEach(x=>x.SetColor(true, false,false));
                    twoGroup.ForEach(x=>x.SetColor(false, false,true));
                    break;
                case LightState.YellowN1:
                    oneGroup.ForEach(x=>x.SetColor(true, true,false));
                    twoGroup.ForEach(x=>x.SetColor(false, true,false));
                    p1.canRun = false;
                    p2.canRun = false;
                    break;
                case LightState.N2:
                    p1.canRun = false;
                    p2.canRun = true;
                    oneGroup.ForEach(x=>x.SetColor(false, false,true));
                    twoGroup.ForEach(x=>x.SetColor(true, false,false));
                    break;
                case LightState.YellowN2:
                    oneGroup.ForEach(x=>x.SetColor(false, true,false));
                    twoGroup.ForEach(x=>x.SetColor(true, true,false));
                    p1.canRun = false;
                    p2.canRun = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
