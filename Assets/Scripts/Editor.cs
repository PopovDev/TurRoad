using System;
using JetBrains.Annotations;
using UnityEngine;

public class Editor : MonoBehaviour
{
    public InputManager inputManager;
    public StructureManager structureManager;
    public RoadManager roadManager;
    [UsedImplicitly]
    public void SpecialPlacementHandler()
    {
        inputManager.ClearEvents();
        inputManager.OnMouseClick += pos => ProcessInputAndCall(structureManager.PlaceSpecial, pos);
    }
    [UsedImplicitly]
    public void HousePlacementHandler()
    {
        inputManager.ClearEvents();
        inputManager.OnMouseClick += pos => ProcessInputAndCall(structureManager.PlaceHouse, pos);
    }
    [UsedImplicitly]
    public void RoadPlacementHandler()
    {
        inputManager.ClearEvents();
        inputManager.OnMouseClick += pos => ProcessInputAndCall(roadManager.PlaceRoad, pos);
        inputManager.OnMouseUp += roadManager.FinishPlacingRoad;
    }
    [UsedImplicitly]
    public void ClearInputActions() => inputManager.ClearEvents();
    private static void ProcessInputAndCall(Action<Vector3Int> callback, Ray ray)
    {
        var result = RaycastGround(ray);
        if (!result.HasValue) return;
        callback.Invoke(result.Value);
    }

    private static Vector3Int? RaycastGround(Ray ray)
    {
        if (!Physics.Raycast(ray, out var hit, Mathf.Infinity)) return null;
        return Vector3Int.RoundToInt(hit.point);;
    }
}
