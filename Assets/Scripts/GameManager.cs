using SimpleCity.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    public CameraMovement cameraMovement;
    public InputManager inputManager;

    [Range(0.1f,20)]
    public float sceneSpeed;
    
    private void Start()
    {
    

        
    }
    private void FixedUpdate()
    {
        Time.timeScale = sceneSpeed;
    }

    private void Update() => cameraMovement.MoveCamera(inputManager.CameraMovementVector,sceneSpeed);
}
