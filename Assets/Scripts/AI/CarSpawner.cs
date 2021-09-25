using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CarSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject[] carPrefabs;
    private void Start() => Instantiate(carPrefabs[Random.Range(0, carPrefabs.Length)], transform);
}
