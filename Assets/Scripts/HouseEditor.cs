using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private Planner.Plan _selectedCar;

    private void Start()
    {
        carDB.onClick.AddListener(() =>
        {
            if (_selectedCar == null) return;
            _selectedCar.carCount--;
            carNumText.text = $"{_selectedCar.carCount}";
        });
        carIb.onClick.AddListener(() =>
        {
            if (_selectedCar == null) return;
            _selectedCar.carCount++;
            carNumText.text = $"{_selectedCar.carCount}";
        });
    }

    public void HouseHover(Ray ray, GameObject mark)
    {
       // mark.SetActive(false);
       // if (!Physics.Raycast(ray, out var hit, Mathf.Infinity)) return;
       // var f = hit.collider.gameObject.GetComponentInParent<StructureModel>();
       // if (f == null) return;
       // if (planner.plans.All(x => x.house != f)) return;
       // mark.SetActive(true);
       // mark.transform.position = f.transform.position;
       // _selectedCar = planner.plans.First(x => x.house == f);
    }
    public void OpenSettings(MenuHandler menuHandler, Action f)
    {
      //  if(_selectedCar==null) return;
      //  
//
      //  menuHandler.gameObject.SetActive(true);
      //  f();
      //  carNumText.text = $"{_selectedCar.carCount}";
      //  hPanel.SetActive(true);
        
    }
}