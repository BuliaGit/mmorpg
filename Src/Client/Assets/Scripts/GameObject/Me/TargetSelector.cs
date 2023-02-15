using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSelector : MonoSingleton<TargetSelector> 
{
    Projector projector;

    bool actived = false;

    Vector3 center;
    private float range;
    private float size;
    Vector3 offset = new Vector3(0, 2, 0);

    protected Action<Vector3> onSelectPoint;

    protected override void OnStart()
    {
        projector = GetComponentInChildren<Projector>();
        projector.gameObject.SetActive(actived);
    }

    public void SetActive(bool active)
    {
        this.actived = active;
        if (projector == null) return;

        projector.gameObject.SetActive(actived);
        projector.orthographicSize = size * 0.5f;
    }

    void Update()
    {
        if (!actived) return;
        if (projector == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 100, LayerMask.GetMask("Terrain")))
        {
            Vector3 hitPoint = hitInfo.point;
            Vector3 dist = hitPoint - center;

            if(dist.magnitude > range)
            {
                hitPoint = center + dist.normalized * range;
            }

            projector.gameObject.transform.position = hitPoint + offset;
            if (Input.GetMouseButtonDown(0))
            {
                onSelectPoint(hitPoint);
                SetActive(false);
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            SetActive(false);
        }
    }

    public static void ShowSelector(Vector3Int center,int range,int size,Action<Vector3> onPositionSelected)
    {
        if (TargetSelector.Instance == null) return;
        TargetSelector.Instance.center = GameObjectTool.LogicToWorld(center);
        TargetSelector.Instance.range = GameObjectTool.LogicToWorld(range);
        TargetSelector.Instance.size = GameObjectTool.LogicToWorld(size);
        TargetSelector.Instance.onSelectPoint = onPositionSelected;
        TargetSelector.Instance.SetActive(true);
    }
}
