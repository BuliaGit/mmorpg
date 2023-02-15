using Managers;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public Collider minimapBoudingbox;

    private void Start()
    {
        MinimapManager.Instance.UpdateMinimap(minimapBoudingbox);
    }
}  
