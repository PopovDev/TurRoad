using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HouseEditor : MonoBehaviour
{
    [SerializeField] private Text carNumText;
    [SerializeField] private Button carDB;
    [SerializeField] private Button carIb; 
    [SerializeField] private Planner planner;
    [SerializeField] private GameObject hPanel;
    [SerializeField] private Slider sliderS;
    [SerializeField] private Text fText;
    [SerializeField] private Button stBtn;
    [SerializeField] private Text stBtnText;
    
    
    private Planner.Plan _selectedCar;
    private GameObject _mark;
    private void Start()
    {
        carDB.onClick.AddListener(() =>
        {
            if (_selectedCar == null) return;
            _selectedCar.carCount--;
            if (_selectedCar.carCount < 0) _selectedCar.carCount = 0;
        });
        carIb.onClick.AddListener(() =>
        {
            if (_selectedCar == null) return;
            _selectedCar.carCount++;
        });
        sliderS.onValueChanged.AddListener((v) =>
        {
            _selectedCar.interval = v;
            fText.text = $"{v:F1}";
        });
        stBtn.onClick.AddListener(() =>
        {
            _selectedCar.stop = !_selectedCar.stop;
            stBtnText.text = _selectedCar.stop ? @"Запустити" : @"Зупиниити";
        });
    }

    public void HouseHover(Ray ray, GameObject mark)
    {
        _mark = mark;
        mark.SetActive(false);
        if (!Physics.Raycast(ray, out var hit, Mathf.Infinity)) return;
        var f = hit.collider.gameObject.GetComponentInParent<StructureModel>();
        if (f == null) return;
        if (planner.plans.All(x => x.House != f)) return;
        mark.SetActive(true);
        mark.transform.position = f.transform.position;
        _selectedCar = planner.plans.First(x => x.House == f);
    }
    [UsedImplicitly]
    public void OpenSettings(MenuHandler menuHandler, Action f)
    {
        if(_selectedCar==null) return;
        menuHandler.gameObject.SetActive(true);
        f();
        carNumText.text = $"{_selectedCar.carCount}";
        hPanel.SetActive(true);
        _mark.SetActive(true);
        sliderS.value = _selectedCar.interval;
        stBtn.onClick.Invoke();
        stBtn.onClick.Invoke();
    }
    [UsedImplicitly]
    public void ExitSettings()
    {
        _selectedCar = null;
        hPanel.SetActive(false);
    }

    private void Update()
    {
        if(_selectedCar== null) return;
        _mark.transform.position = _selectedCar.House.transform.position;
        carNumText.text = $"{_selectedCar.carCount}";
    }
}