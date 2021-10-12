using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AI;
using AI.TrafficLights;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LightEditor : MonoBehaviour
{
    [SerializeField] private GameObject lightPanel;

    
    [SerializeField] private Button lightStopBtn;
    [SerializeField] private Text lightStopBtnText;
    [Space][SerializeField] private Slider n1S;
    [SerializeField] private Text n1SText;
    [Space][SerializeField] private Slider n2S;
    [SerializeField] private Text n2SText;
    [Space][SerializeField] private Slider n3S;
    [SerializeField] private Text n3SText;
    [Space][SerializeField] private Slider n4S;
    [SerializeField] private Text n4SText;
    
    private LightController _selectedLights;
    private LightWorker.LightSetting _selectedLightsSetting;
    private GameObject _mark;
    [SerializeField]
    private LightWorker lightWorker;

    private void Start()
    {
        lightStopBtn.onClick.AddListener(() =>
        {
           var fd =  _selectedLights.Stopped =! _selectedLights.Stopped;
           lightStopBtnText.text = fd ?@"Запустить" : @"Заморозить";
        });
        n1S.onValueChanged.AddListener((g) =>
        {
            if (_selectedLightsSetting == null) return;
            _selectedLightsSetting.n1Time = g;
            n1SText.text = $"{g:F2}";
            n1S.SetValueWithoutNotify(g);
        });
        n2S.onValueChanged.AddListener((g) =>
        {
            if (_selectedLightsSetting == null) return;
            _selectedLightsSetting.yellowOneTime = g;
            n2SText.text = $"{g:F2}";
            n2S.SetValueWithoutNotify(g);
        });
        n3S.onValueChanged.AddListener((g) =>
        {
            if (_selectedLightsSetting == null) return;
            _selectedLightsSetting.n2Time = g;
            n3SText.text = $"{g:F2}";
            n3S.SetValueWithoutNotify(g);
        });
        n4S.onValueChanged.AddListener((g) =>
        {
            if (_selectedLightsSetting == null) return;
            _selectedLightsSetting.yellowTwoTime = g;
            n4SText.text = $"{g:F2}";
            n4S.SetValueWithoutNotify(g);
        });
    }

    public void LightHover(Ray ray, GameObject m)
    {
        _mark = m;
        m.SetActive(false);
        if (!Physics.Raycast(ray, out var hit, Mathf.Infinity)) return;
        if (!hit.collider.gameObject.TryGetComponent(out SmartRoad a)) return;
        var ab = a.gameObject.GetComponentInChildren<LightController>();
        if (ab == null) return;
        m.SetActive(true);
        m.transform.position = a.transform.position;
        _selectedLights = ab;

    }
    public void OpenSettings(MenuHandler menuHandler, Action f)
    {
        if(_selectedLights==null) return;
        menuHandler.gameObject.SetActive(true);
        f();
        lightPanel.SetActive(true);
        _mark.SetActive(true);
        _selectedLightsSetting = lightWorker.lightSettings.FirstOrDefault(x => x.control == _selectedLights);
        n1S.onValueChanged.Invoke(_selectedLightsSetting!.n1Time);
        n2S.onValueChanged.Invoke(_selectedLightsSetting!.yellowOneTime);
        n3S.onValueChanged.Invoke(_selectedLightsSetting!.n2Time);
        n4S.onValueChanged.Invoke(_selectedLightsSetting!.yellowTwoTime);
        lightStopBtnText.text = _selectedLights.Stopped ?@"Запустить" : @"Заморозить";
    }
    [UsedImplicitly]
    public void ExitSettings()
    {
        _selectedLights = null;
        lightPanel.SetActive(false);
    }

    private void Update()
    {
        if(_selectedLights== null) return;

        _mark.transform.position = _selectedLights.transform.position;
    }
}
