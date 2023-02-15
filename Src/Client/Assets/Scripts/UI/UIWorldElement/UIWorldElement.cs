using UnityEngine;

/// <summary>
/// 主要解决UI在3D世界的朝向
/// </summary>
public class UIWorldElement : MonoBehaviour
{
    public Transform owner;

    public float height = 2.2f;


    // Update is called once per frame
    void Update()
    {
        if (owner != null)
        {
            this.transform.position = owner.position + Vector3.up * height;
        }

        if (Camera.main != null)
            this.transform.forward = Camera.main.transform.forward;
    }
}
