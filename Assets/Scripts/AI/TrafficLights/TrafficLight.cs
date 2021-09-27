using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TrafficLight : MonoBehaviour
{
    [SerializeField] private Material red;
    [SerializeField] private Material yellow;
    [SerializeField] private Material green;
    [SerializeField] private Material gray;
    [SerializeField] private GameObject lightsPart;
    [SerializeField] private Transform normal;
    [SerializeField] private Transform up;
    [SerializeField] private bool isUp;

    private void Update()
    {
        if (isUp)
        {
            lightsPart.transform.position = up.position;
            lightsPart.transform.rotation = up.rotation;
        }
        else
        {
            lightsPart.transform.position = normal.position;
            lightsPart.transform.rotation = normal.rotation;
        }
     
    }
}