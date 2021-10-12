using System;
using AI;
using JetBrains.Annotations;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Editor : MonoBehaviour
{
    public InputManager inputManager;
    private CarEditor _carEditor;
    private LightEditor _lightEditor;
    public StructureManager structureManager;
    public RoadManager roadManager;
    public AiDirector aiDirector;
    public GameObject greenMark;
    public GameObject redMark;

    private void Start()
    {
        _carEditor = GetComponent<CarEditor>();
        _lightEditor = GetComponent<LightEditor>();
    }

    [UsedImplicitly]
    public void SpecialPlacementHandler()
    {
        ClearInputActions();
        inputManager.OnMouseClick += pos => ProcessInputAndCall(structureManager.PlaceSpecial, pos);
        inputManager.OnMouseHover += pos =>
            ProcessInputAndCall(i => structureManager.PlaceHover(i, greenMark, redMark, true), pos);
        inputManager.OnMouseUp += structureManager.FinishPlace;
    }

    [UsedImplicitly]
    public void HousePlacementHandler()
    {
        ClearInputActions();
        inputManager.OnMouseClick += pos => ProcessInputAndCall(structureManager.PlaceHouse, pos);
        inputManager.OnMouseHover += pos =>
            ProcessInputAndCall(i => structureManager.PlaceHover(i, greenMark, redMark, false), pos);
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
        inputManager.OnMouseClick += pos => ProcessInputAndCall((i) => { roadManager.PlaceRoad(i); }, pos);
        inputManager.OnMouseUp += roadManager.FinishPlacingRoad;
    }

    [UsedImplicitly]
    public void ClearInputActions()
    {
        inputManager.ClearEvents();
        greenMark.SetActive(false);
        redMark.SetActive(false);
        greenMark.transform.position =Vector3.zero;
        redMark.transform.position =Vector3.zero;
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
    }
    [UsedImplicitly]
    public void CarEditHandler(MenuHandler carMenu)
    {
        ClearInputActions();
        inputManager.OnMouseHover += pos => _carEditor.CarHover(pos, greenMark);
        inputManager.OnMouseUp += () => _carEditor.OpenSettings(carMenu,ClearInputActions);
    }
    [UsedImplicitly]
    public void LightEditHandler(MenuHandler lightMenu)
    {
        ClearInputActions();
        inputManager.OnMouseHover += pos => _lightEditor.LightHover(pos, greenMark);
        inputManager.OnMouseUp += () => _lightEditor.OpenSettings(lightMenu,ClearInputActions);
    }
}