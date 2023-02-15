using Common.Data;
using Services;
using UnityEngine;

public class TeleporterObject : MonoBehaviour
{
    public int ID;

    Mesh mesh = null;

    // Use this for initialization
    void Start()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;
    }


    void OnTriggerEnter(Collider other)
    {
        PlayerInputController playerInputController = other.GetComponent<PlayerInputController>();
        if (playerInputController != null && playerInputController.isActiveAndEnabled) //是否处于激活状态
        {
            TeleporterDefine td = DataManager.Instance.Teleporters[this.ID];
            if (td == null)
            {
                Debug.LogFormat("TeleporterObject: Character[{0}] Enter TeleporterObject[{1}],but TeleporterDefine not existed ", playerInputController.character.Info.Name, td.ID);
                
                return;
            }

            Debug.LogFormat("TeleporterObject: Character[{0}] Enter TeleporterObject[{1}:{2}]", playerInputController.character.Info.Name, td.ID, td.Name);

            if (td.LinkTo > 0)
            {
                if (DataManager.Instance.Teleporters.ContainsKey(td.LinkTo))
                    MapService.Instance.SendMapTeleport(this.ID);
                else
                    Debug.LogFormat("Teleporter ID:{0}:LinkID{1}error}", td.ID, td.LinkTo);
            }
        }
    }


#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        if (mesh != null)
        {
            Gizmos.DrawWireMesh(this.mesh, transform.position + Vector3.up * transform.localScale.y * 0.5f, transform.rotation, transform.localScale);
        }
        UnityEditor.Handles.color = Color.blue;
        UnityEditor.Handles.ArrowHandleCap(0, transform.position, transform.rotation, 1, EventType.Repaint);
    }
#endif
}
