using Common;
using Common.Data;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelePorterObjectMe : MonoBehaviour {

	public int id;
	private Mesh mesh;

	private void Start()
	{
		mesh = GetComponent<MeshFilter>().sharedMesh;
	}

#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		if(mesh != null)
		{
			Gizmos.DrawWireMesh(mesh,transform.position,transform.rotation,transform.localScale);
		}
		UnityEditor.Handles.color = Color.blue;
		UnityEditor.Handles.ArrowHandleCap(0, transform.position, transform.rotation, 1, EventType.Repaint);
	}
#endif

	private void OnTriggerEnter(Collider other)
	{
		PlayerInputController playerInputControl = other.GetComponent<PlayerInputController>();
		if(playerInputControl != null)
		{
			TeleporterDefine telePorterDefine = DataManager.Instance.Teleporters[id];
			if(telePorterDefine == null)
			{
				Debug.LogFormat("TeleporterObject: Character[{0}] Enter TeleporterObject[{1}],but TeleporterDefine not existed ", playerInputControl.character.Info.Name, id);
				return;
			}
			Debug.LogFormat("TeleporterObject: Character[{0}] Enter TeleporterObject[{1}:{2}]", playerInputControl.character.Info.Name,telePorterDefine.ID,telePorterDefine.Name);
			if (telePorterDefine.LinkTo > 0)
			{
				if (DataManager.Instance.Teleporters.ContainsKey(telePorterDefine.LinkTo))
				{
					MapService.Instance.SendMapTeleport(telePorterDefine.ID);
				}
				else
				{
					Debug.LogFormat("TelePorter ID:{0}--LinkTo{1} Error", telePorterDefine.ID, telePorterDefine.LinkTo);
				}
			}
		}
	}
}
