using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Light3Rt : MonoBehaviour
{
    private void Update()
    {
        var transform1 = transform;
        transform1.eulerAngles = new Vector3(0,-180,0);
    }
}
