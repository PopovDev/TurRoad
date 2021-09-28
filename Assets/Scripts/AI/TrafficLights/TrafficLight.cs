using System;
using System.Collections;
using System.Collections.Generic;
using SVS;
using UnityEngine;
using UnityEngine.Serialization;
[ExecuteInEditMode]
public class TrafficLight : MonoBehaviour
{
    [SerializeField] private Material red;
    [SerializeField] private Material yellow;
    [SerializeField] private Material green;
    [SerializeField] private Material gray;
    [SerializeField] private GameObject lightsPart;
    [SerializeField] private Transform normal;
    [SerializeField] private Transform up;
    private CameraMovement _cam;
    private void Start()
    {
        _cam = FindObjectOfType<CameraMovement>();
        _cam.CamModeChanged+= CamOnCamModeChanged;
    }

    private void CamOnCamModeChanged(bool camUp)
    {
        if (camUp)
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