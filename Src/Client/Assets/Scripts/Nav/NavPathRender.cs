using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavPathRender : MonoSingleton<NavPathRender>
{
    LineRenderer pathRenderer;
    NavMeshPath path;
    void Start()
    {
        pathRenderer = GetComponent<LineRenderer>();
        pathRenderer.enabled = false;
    }

    public void SetPath(NavMeshPath path, Vector3 target)
    {
        this.path = path;
        if (path == null)
        {
            pathRenderer.enabled = false;
            pathRenderer.positionCount = 0;
        }
        else
        {
            pathRenderer.enabled = true;
            pathRenderer.positionCount = path.corners.Length + 1;
            pathRenderer.SetPositions(path.corners);
            pathRenderer.SetPosition(pathRenderer.positionCount - 1, target);
            for (int i = 0; i < pathRenderer.positionCount; i++)
            {
                pathRenderer.SetPosition(i, pathRenderer.GetPosition(i) + Vector3.up * 0.2f);
            }
        }
    }
}
