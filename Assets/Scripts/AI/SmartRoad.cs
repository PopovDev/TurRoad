using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

public class SmartRoad : MonoBehaviour
{
    private readonly Queue<CarAI> _trafficQueue = new Queue<CarAI>();
    public CarAI currentCar;

    [SerializeField]
    private bool pedestrianWaiting = false, pedestrianWalking = false;

    [field: SerializeField] private UnityEvent OnPedestrianCanWalk { get; set; }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag($"Car")) return;
        var car = other.GetComponent<CarAI>();
        if (car == null || car == currentCar || car.IsThisLastPathIndex() != false) return;
        _trafficQueue.Enqueue(car);
        car.Stop = true;
    }

    private void Update()
    {
        if (currentCar != null) return;
        if(_trafficQueue.Count > 0 && pedestrianWaiting == false && pedestrianWalking == false)
        {
            currentCar = _trafficQueue.Dequeue();
            currentCar.Stop = false;
        }else if(pedestrianWalking || pedestrianWaiting)
        {
            OnPedestrianCanWalk?.Invoke();
            pedestrianWalking = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag($"Car")) return;
        var car = other.GetComponent<CarAI>();
        if(car != null) RemoveCar(car);
    }

    private void RemoveCar(Object car)
    {
        if(car == currentCar) currentCar = null;
    }


    public void SetPedestrianFlag(bool val)
    {
        if (val)
        {
            pedestrianWaiting = true;
        }
        else
        {
            pedestrianWaiting = false;
            pedestrianWalking = false;
        }
    }
}
