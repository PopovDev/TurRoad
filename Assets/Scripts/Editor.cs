using System;
using AI;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

public class Editor : MonoBehaviour
{
    public InputManager inputManager;
    public StructureManager structureManager;
    public RoadManager roadManager;
    public AiDirector aiDirector;
    public GameObject greenMark;

    [UsedImplicitly]
    public void SpecialPlacementHandler()
    {
        ClearInputActions();
        inputManager.OnMouseClick += pos => ProcessInputAndCall(structureManager.PlaceSpecial, pos);
        inputManager.OnMouseHover += pos => ProcessInputAndCall(i => structureManager.PlaceHover(i, greenMark), pos);
        inputManager.OnMouseUp += structureManager.FinishPlace;
    }

    [UsedImplicitly]
    public void HousePlacementHandler()
    {
        ClearInputActions();
        inputManager.OnMouseClick += pos => ProcessInputAndCall(structureManager.PlaceHouse, pos);
        inputManager.OnMouseHover += pos => ProcessInputAndCall(i => structureManager.PlaceHover(i, greenMark), pos);
        inputManager.OnMouseUp += structureManager.FinishPlace;
    }

    [UsedImplicitly]
    public void CarPlacementHandler()
    {
        ClearInputActions();
        inputManager.OnMouseClick += pos => ProcessInputAndCall(aiDirector.SpawnCar, pos);
        inputManager.OnMouseHover += pos => ProcessInputAndCall(i => aiDirector.MarkHover(i, greenMark), pos);
        inputManager.OnMouseUp += aiDirector.FinishSpawnCar;
    }

    [UsedImplicitly]
    public void RoadPlacementHandler()
    {
        ClearInputActions();
        inputManager.OnMouseClick += pos => ProcessInputAndCall(roadManager.PlaceRoad, pos);
        inputManager.OnMouseUp += roadManager.FinishPlacingRoad;
    }

    [UsedImplicitly]
    public void ClearInputActions()
    {
        inputManager.ClearEvents();
        greenMark.SetActive(false);
    }

    private static void ProcessInputAndCall(Action<Vector3Int> callback, Ray ray)
    {
        var result = RaycastGround(ray);
        if (!result.HasValue) return;
        callback.Invoke(result.Value);
    }

    private static Vector3Int? RaycastGround(Ray ray)
    {
        if (!Physics.Raycast(ray, out var hit, Mathf.Infinity)) return null;
        return Vector3Int.RoundToInt(hit.point);
        ;
    }
}