﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CarAI : MonoBehaviour
{
    private List<Vector3> _path;
    [SerializeField] private float arriveDistance = .3f, lastPointArriveDistance = .1f;
    [SerializeField] private float turningAngleOffset = 5;
    [SerializeField] private Vector3 currentTargetPosition;
    [SerializeField] private GameObject raycastStartingPoint;
    [SerializeField] private float collisionRaycastLength = 0.1f;

    internal bool IsThisLastPathIndex() => _index >= _path.Count - 1;

    private int _index;

    private bool _stop;

    private bool _collisionStop;

    public bool Stop
    {
        get => _stop || _collisionStop;
        set => _stop = value;
    }

    private UnityEvent<Vector2> OnDrive { get; }= new UnityEvent<Vector2>();

    private void Start()
    {
        OnDrive.AddListener(GetComponent<CarController>().Move);
        if (_path == null || _path.Count == 0)
            Stop = true;
        else
            currentTargetPosition = _path[_index];
    }

    public void SetPath(List<Vector3> p)
    {
        if (p.Count == 0)
        {
            Destroy(gameObject);
            return;
        }
        
        _path = p;
        _index = 0;
        currentTargetPosition = _path[_index];

        var relativePoint = transform.InverseTransformPoint(_path[_index + 1]);

        var angle = Mathf.Atan2(relativePoint.x, relativePoint.z) * Mathf.Rad2Deg;
        
        transform.rotation = Quaternion.Euler(0, angle, 0);
        Stop = false;
    }

    private void Update()
    {
        CheckIfArrived();
        Drive();
        CheckForCollisions();
    }

    private void CheckForCollisions()
    {
        _collisionStop = Physics.Raycast(raycastStartingPoint.transform.position, transform.forward,
            collisionRaycastLength, 1 << gameObject.layer);
    }

    private void Drive()
    {
        if (Stop)
        {
            OnDrive?.Invoke(Vector2.zero);
        }
        else
        {
            var relativepoint = transform.InverseTransformPoint(currentTargetPosition);
            var angle = Mathf.Atan2(relativepoint.x, relativepoint.z) * Mathf.Rad2Deg;
            var rotateCar = 0;
            if (angle > turningAngleOffset)
                rotateCar = 1;
            else if (angle < -turningAngleOffset) rotateCar = -1;

            OnDrive?.Invoke(new Vector2(rotateCar, 1));
        }
    }

    private void CheckIfArrived()
    {
        if (Stop) return;
        
        var distanceToCheck = arriveDistance;
        if (_index == _path.Count - 1) distanceToCheck = lastPointArriveDistance;

        if (Vector3.Distance(currentTargetPosition, transform.position) < distanceToCheck)
        {
            SetNextTargetIndex();
        }
    }

    private void SetNextTargetIndex()
    {
        _index++;
        if (_index >= _path.Count)
        {
            Stop = true;
            Destroy(gameObject);
        }
        else
        {
            currentTargetPosition = _path[_index];
        }
    }
}