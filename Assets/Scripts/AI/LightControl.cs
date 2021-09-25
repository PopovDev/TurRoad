using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class LightControl : MonoBehaviour
{
    [SerializeField]
    private bool canRun;

    private SmartRoad _road;

    private void Start()
    {
        _road = GetComponentInParent<SmartRoad>();
    }

    
    private void OnTriggerExit(Collider other)
    {
        _road.exited.Add(other.gameObject);
        _road.exited.RemoveAll(x => x==null);
    }

    private void OnTriggerStay(Collider other)
    {
        if(_road.exited.Contains(other.gameObject)) return;
        var ai = other.GetComponent<CarAI>();
        if (ai == null) return;
        if (_road.currentCar == other.GetComponent<CarAI>())
        {
            ai.Stop = false;
            return;
        }
        
        ai.Stop = canRun;
    }

}
