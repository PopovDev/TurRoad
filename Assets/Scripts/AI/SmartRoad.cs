using System.Collections.Generic;
using UnityEngine;

public class SmartRoad : MonoBehaviour
{
    private readonly Queue<CarAI> _trafficQueue = new Queue<CarAI>();
    public CarAI currentCar;
    public List<GameObject> exited = new List<GameObject>();
    
    private void OnTriggerEnter(Collider other)
    {
        
        if (!other.CompareTag($"Car")) return;
        var car = other.GetComponent<CarAI>();
        if (car == null || car == currentCar || car.IsThisLastPathIndex()) return;
        _trafficQueue.Enqueue(car);
        car.Stop = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag($"Car")) return;
        var car = other.GetComponent<CarAI>();
        if (car == null) return;
        if(car == currentCar) currentCar = null;
    }

}
