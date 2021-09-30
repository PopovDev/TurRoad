using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class MenuHandler : MonoBehaviour
{ 
    [Serializable]
    private class KeyMh
    {
        public KeyCode key;
        [CanBeNull]public MenuHandler menu;
        [CanBeNull] public UnityEvent func;
    }
    [SerializeField]
    private List<KeyMh> handlers; 
    
    private void OnEnable()
    {
        var obj = FindObjectsOfType<MenuHandler>();
        foreach (var menuHandler in obj.Where(x=>x!=this))
            menuHandler.gameObject.SetActive(false);
    }
    private void Update()
    {
        var inv = handlers.Where(x => Input.GetKeyDown(x.key));
        foreach (var h in inv)
        {
            if (h.menu != null) h.menu.gameObject.SetActive(true);
            h.func?.Invoke();
        }
       
    }
}
