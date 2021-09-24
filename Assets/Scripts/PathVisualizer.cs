using SimpleCity.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathVisualizer : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    private AiAgent _currentAgent;

    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 0;
    }

    public void ShowPath(List<Vector3> path, AiAgent agent, Color color)
    {
        ResetPath();
        _lineRenderer.positionCount = path.Count;
        _lineRenderer.startColor = color;
        _lineRenderer.endColor = color;
        for (var i = 0; i < path.Count; i++)
            _lineRenderer.SetPosition(i, path[i] + new Vector3(0, agent.transform.position.y, 0));
        _currentAgent = agent;
        _currentAgent.OnDeath += ResetPath;
    }

    public void ResetPath()
    {
        if(_lineRenderer != null) _lineRenderer.positionCount = 0;
        if(_currentAgent != null) _currentAgent.OnDeath -= ResetPath;
        _currentAgent = null;
    }
}
