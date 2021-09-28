using System;
using JetBrains.Annotations;
using UnityEngine;
public class LightController : MonoBehaviour
{
    [SerializeField] private TrafficGate p1;
    [SerializeField] private TrafficGate p2;
    
    [SerializeField] public LightState state;
    
    [SerializeField] private TrafficLight one;
    [SerializeField] private TrafficLight two;
    [SerializeField] private TrafficLight three;
    [SerializeField] [CanBeNull] private TrafficLight four;

    public enum LightState
    {
        N1,
        N2,
        Yellow,
        Red
    }

    
    
    private void Update()
    {

        switch (state)
        {
            case LightState.N1:
                p1.canRun = true;
                p2.canRun = false;
                break;
            case LightState.N2:
                p1.canRun = false;
                p2.canRun = true;
                break;
            case LightState.Yellow:
                p1.canRun = false;
                p2.canRun = false;
                break;
            case LightState.Red:
                p2.canRun = false;
                p2.canRun = false;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
