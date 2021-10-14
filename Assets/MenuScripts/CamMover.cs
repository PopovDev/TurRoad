using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class CamMover : MonoBehaviour
{
    [SerializeField]
    private Vector3 baseRt;
    
    private void Update()
    {
        var x = (Input.mousePosition.x-Screen.width/2f)/Screen.width*2*2;
        var y = (Input.mousePosition.y-Screen.height/2f)/Screen.height*2*2;
        transform.eulerAngles = baseRt+ new Vector3(y,-x);
    }
}
