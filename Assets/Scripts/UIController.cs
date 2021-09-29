using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Space]
    [SerializeField]
    private Text modeText;
    [SerializeField]
    private string modeTextBase;

    public event Action<bool> OnTextChanged;

    private void Start()
    {
        
    }
    
}
