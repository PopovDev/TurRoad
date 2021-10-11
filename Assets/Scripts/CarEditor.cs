using System;
using System.Collections;
using System.Collections.Generic;
using AI;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class CarEditor : MonoBehaviour
{
    private CarController _selectedCar;
    [SerializeField]
    private Camera mainCam;
    private bool InCar { get; set; }
    [SerializeField] private GameObject carPanel;
    public event Action<bool> CamModeChanged;
    public void CarHover(Ray ray, GameObject mark)
    {
        mark.SetActive(false);
        if (!Physics.Raycast(ray, out var hit, Mathf.Infinity)) return;
        if (!hit.collider.gameObject.TryGetComponent(out CarController a)) return;
        _selectedCar = a;
        mark.SetActive(true);
        mark.transform.position = hit.point;

    }
    public void OpenSettings(MenuHandler menuHandler, Action f)
    {
        if(_selectedCar==null) return;
        
        mainCam.enabled = false;
        _selectedCar.cam.enabled = true;
        menuHandler.gameObject.SetActive(true);
        f();
        InCar = true;
        carPanel.SetActive(true);
        CamModeChanged?.Invoke(false);
    }
    [UsedImplicitly]
    public void ExitSettings()
    {
        if(_selectedCar!=null) _selectedCar.cam.enabled = false;
        _selectedCar = null;
        mainCam.enabled = true;
        InCar = false;
        carPanel.SetActive(false);
        CamModeChanged?.Invoke(true);
    }

    private void Update()
    {
        if (!InCar) return;
        if (_selectedCar == null) ExitSettings();
    }
}
