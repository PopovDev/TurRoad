using SimpleCity.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public CameraMovement cameraMovement;
    public RoadManager roadManager;
    public InputManager inputManager;
    public UIController uiController;
    public StructureManager structureManager;
    [Range(0.1f,20)]
    public float sceneSpeed;
    
    private void Start()
    {
        BindMenuKeys();

        
    }
    private void FixedUpdate()
    {
        Time.timeScale = sceneSpeed;
        Debug.Log(inputManager.CameraMovementVector);
    }
    private void BindMenuKeys()
    {
        inputManager.OnE += RoadPlacementHandler;
        inputManager.OnQ += HousePlacementHandler;
        inputManager.OnR += SpecialPlacementHandler;
        inputManager.OnEscape += HandleEscape;
    }
    


    private void HandleEscape()
    {
        ClearInputActions();
        BindMenuKeys();
    }


    private void SpecialPlacementHandler()
    {
        ClearInputActions();
        inputManager.OnMouseClick += pos => ProcessInputAndCall(structureManager.PlaceSpecial, pos);
        inputManager.OnEscape += HandleEscape;
    }

    private void HousePlacementHandler()
    {
        ClearInputActions();
        inputManager.OnMouseClick += pos => ProcessInputAndCall(structureManager.PlaceHouse, pos);
        inputManager.OnEscape += HandleEscape;
    }

    private void RoadPlacementHandler()
    {
        ClearInputActions();
        inputManager.OnMouseClick += pos => ProcessInputAndCall(roadManager.PlaceRoad, pos);
        inputManager.OnMouseUp += roadManager.FinishPlacingRoad;
        inputManager.OnEscape += HandleEscape;
    }

    private void ClearInputActions() => inputManager.ClearEvents();

    private static void ProcessInputAndCall(Action<Vector3Int> callback, Ray ray)
    {
        var result = RaycastGround(ray);
        if (result.HasValue)
            callback.Invoke(result.Value);
    }

    private static Vector3Int? RaycastGround(Ray ray)
    {
        if (!Physics.Raycast(ray, out var hit, Mathf.Infinity)) return null;
        var positionInt = Vector3Int.RoundToInt(hit.point);
        return positionInt;
    }


    private void Update() => cameraMovement.MoveCamera(inputManager.CameraMovementVector,sceneSpeed);
}
