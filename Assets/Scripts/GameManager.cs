﻿using SimpleCity.AI;
using SVS;
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
    public ObjectDetector objectDetector;

    private void Start()
    {
        uiController.OnRoadPlacement += RoadPlacementHandler;
        uiController.OnHousePlacement += HousePlacementHandler;
        uiController.OnSpecialPlacement += SpecialPlacementHandler;
        inputManager.OnEscape += HandleEscape;
    }

    private void HandleEscape()
    {
        ClearInputActions();
        uiController.ResetButtonColor();
        inputManager.OnMouseClick += TrySelectingAgent;
    }

    private static void TrySelectingAgent(Ray ray)
    {
        var hitObject = ObjectDetector.RaycastAll(ray);
        if (hitObject == null) return;
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
        inputManager.OnMouseHold += pos => ProcessInputAndCall(roadManager.PlaceRoad, pos);
        inputManager.OnEscape += HandleEscape;
    }

    private void ClearInputActions() => inputManager.ClearEvents();

    private void ProcessInputAndCall(Action<Vector3Int> callback, Ray ray)
    {
        var result = objectDetector.RaycastGround(ray);
        if (result.HasValue)
            callback.Invoke(result.Value);
    }



    private void Update() => cameraMovement.MoveCamera(inputManager.CameraMovementVector);
}
