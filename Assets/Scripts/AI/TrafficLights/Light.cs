using System;
using UnityEngine;
public class Light : MonoBehaviour
{
    [SerializeField] private LightControl p1;
    [SerializeField] private LightControl p2;
    [SerializeField] public LightState state;
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
