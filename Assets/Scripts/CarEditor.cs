using System;
using AI;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class CarEditor : MonoBehaviour
{
    private CarController _selectedCar;
    [SerializeField]
    private Camera mainCam;
    private bool InCar { get; set; }
    [SerializeField] private GameObject carPanel;
    public event Action<bool> CamModeChanged;
    
    [SerializeField] private Button carStopBtn;
    [SerializeField] private Text carStopBtnText;
    [SerializeField] private Slider carSpeedSlider;
    [SerializeField] private Text carSpeedSliderText;

    private void Start()
    {
        carStopBtn.onClick.AddListener(() =>
        {
            if (_selectedCar == null) return;
            _selectedCar.Stop = !_selectedCar.Stop;
            carStopBtnText.text = _selectedCar.Stop ? @"Їхати далі" : @"Зупинитися";
        });
        carSpeedSlider.onValueChanged.AddListener(CarSpeed);
    }

    private void CarSpeed(float v)
    {
        if (_selectedCar == null) return;
        _selectedCar.speedScale = v;
        carSpeedSliderText.text = $"{v:F2}";
    }

    public void CarHover(Ray ray, GameObject mark)
    {
        mark.SetActive(false);
        if (!Physics.Raycast(ray, out var hit, Mathf.Infinity)) return;
        if (!hit.collider.gameObject.TryGetComponent(out CarController a)) return;
        _selectedCar = a;
        mark.SetActive(true);
        mark.transform.position = a.transform.position;
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
        CarSpeed(_selectedCar.speedScale);
        carSpeedSlider.value = _selectedCar.speedScale;
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
