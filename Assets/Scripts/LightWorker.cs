
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AI.TrafficLights;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;


public class LightWorker : MonoBehaviour
{
    [Serializable]
    public class LightSetting
    {
        public LightController control;
        public float changed;
        public float n1Time=10;
        public float yellowOneTime = 2;
        public float n2Time =10;
        public float yellowTwoTime = 2;
        public bool working = true;
        public void Work()
        {
            if(!working) return;
            switch (control.state)
            {
                case LightController.LightState.N1:
                    if (Time.time - changed > n1Time)
                    {
                        control.state = LightController.LightState.YellowN1;
                        changed = Time.time;
                        control.Changed();
                    }
                    break;
                case LightController.LightState.YellowN1:
                    if (Time.time - changed > yellowOneTime)
                    {
                        control.state = LightController.LightState.N2;
                        changed = Time.time;
                        control.Changed();
                    }
                    break;
                case LightController.LightState.N2:
                    if (Time.time - changed > n2Time)
                    {
                        changed = Time.time;
                        control.state = LightController.LightState.YellowN2;
                        control.Changed();
                    }
                    break;
                case LightController.LightState.YellowN2:
                    if (Time.time - changed > yellowTwoTime)
                    {
                        changed = Time.time;
                        control.state = LightController.LightState.N1;
                        control.Changed();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    [SerializeField]
    public List<LightSetting> lightSettings;
    private IEnumerator Check() 
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            if(this==null) yield break;
            foreach (var controller in FindObjectsOfType<LightController>().Where(controller => lightSettings.All(x => x.control != controller)))
                lightSettings.Add(new LightSetting
                {
                    control = controller
                });


        }
    }
    private IEnumerator Work() 
    {
        while (true)
        {
            lightSettings.RemoveAll(x => x.control == null);
            foreach (var g in lightSettings) g.Work();
            yield return new WaitForSecondsRealtime(0.01f);
            if(this==null) yield break;
        }
    }

    private void Start()
    {
        StartCoroutine(Check());
        StartCoroutine(Work());
        
    }
}
