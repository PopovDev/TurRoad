using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
public class LightController : MonoBehaviour
{
    [SerializeField] private TrafficGate p1;
    [SerializeField] private TrafficGate p2;
    [SerializeField] private LightState state;
    [SerializeField] private List<TrafficLight> oneGroup;
    [SerializeField] private  List<TrafficLight> twoGroup;

    private enum LightState
    {
        N1,
        Yellow,
        N2
    }
    private void FixedUpdate()
    {

        switch (state)
        {
            case LightState.N1:
                p1.canRun = true;
                p2.canRun = false;
                oneGroup.ForEach(x=>x.SetColor(true, false,false));
                twoGroup.ForEach(x=>x.SetColor(false, false,true));
                break;
            case LightState.N2:
                p1.canRun = false;
                p2.canRun = true;
                oneGroup.ForEach(x=>x.SetColor(false, false,true));
                twoGroup.ForEach(x=>x.SetColor(true, false,false));

                break;
            case LightState.Yellow:
                oneGroup.ForEach(x=>x.SetColor(false, true,false));
                twoGroup.ForEach(x=>x.SetColor(false, true,false));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
