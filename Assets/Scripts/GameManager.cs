using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Text speedText;
    [SerializeField] private Slider speedSlider;
    private bool _paused;
    public void Pause()
    {
        _paused =!_paused;
        SetSceneSpeed(false);
    }
    [UsedImplicitly]
    public void SetSceneSpeed(bool set1)
    {
        var t = set1 switch
        {
            true => 0.525f,
            false => speedSlider.value
        };
        Time.timeScale = (t <= 0.5f ? t : (t - 0.5f) * 20 + 0.5f);
        if (_paused)
        {
            speedText.text = @"Пауза";
            Time.timeScale = 0;
        }
        else
        {
            speedText.text = $"x{Time.timeScale:0.00}";
        }
       
        speedSlider.value = t;
    }

    private void Start() => SetSceneSpeed(true);
}
