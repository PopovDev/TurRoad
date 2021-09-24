using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDetector : MonoBehaviour
{
    public LayerMask groundMask;

    public Vector3Int? RaycastGround(Ray ray)
    {
        if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, groundMask)) return null;
        var positionInt = Vector3Int.RoundToInt(hit.point);
        return positionInt;
    }

    public static GameObject RaycastAll(Ray ray)
    {
        return Physics.Raycast(ray, out var hit, Mathf.Infinity) ? hit.transform.gameObject : null;
    }
}
