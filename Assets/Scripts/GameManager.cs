using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public CameraMovement cameraMovement;
    [Range(0f,20)]
    public float sceneSpeed;
    private void FixedUpdate() => Time.timeScale = sceneSpeed;
}
