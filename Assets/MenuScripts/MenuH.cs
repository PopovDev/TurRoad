using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MenuH : MonoBehaviour
{
    public GameObject mainMenu;
    public Button menuExit;
    public Button menuAuthors;
    public Button menuInstruction;
    public Button menuStart;
    [Space] public GameObject pnAuthors;
    public Button pnAuthorsBack;
    [Space] public GameObject startMenu;
    public Button backToMenu;
    public Button createNewSim;
    public Button loadSim;
    public GameObject pnLoadSim;
    public Button pnLoadSimBack;
    public Button loadSimH;
    public InputField loadSimHInputField;
    private void Start()
    {
        menuExit.onClick.AddListener(Application.Quit);
        menuAuthors.onClick.AddListener(() => pnAuthors.SetActive(true));
        pnAuthorsBack.onClick.AddListener(() => pnAuthors.SetActive(false));
        menuInstruction.onClick.AddListener(() =>
            Application.OpenURL("https://github.com/PopovDev/TurRoad/blob/master/Instructions.md"));
        menuStart.onClick.AddListener(() =>
        {
            mainMenu.SetActive(false);
            startMenu.SetActive(true);
        });
        backToMenu.onClick.AddListener(() =>
        {
            mainMenu.SetActive(true);
            startMenu.SetActive(false);
        });
        loadSim.onClick.AddListener(() => pnLoadSim.SetActive(true));
        pnLoadSimBack.onClick.AddListener(() => pnLoadSim.SetActive(false));
        loadSimH.onClick.AddListener(() =>
        {
            try
            {
              var g =   JsonConvert.DeserializeObject<MapSaver.Save>(File.ReadAllText(loadSimHInputField.text));
              MapSaver.Path = loadSimHInputField.text;
              MapSaver.LoadFromFile = true;
              SceneManager.LoadScene(1, LoadSceneMode.Single);

            }
            catch (Exception e)
            {
                Debug.Log(e);
               
            }
           
            
        });
    }
}