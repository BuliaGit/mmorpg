using Managers;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class UIMiniMapMe : MonoBehaviour {

	public Image miniMap;
	public Image arrow;
	public Text mapName;
	private Collider miniMapColliderBox;
	private Transform playerTF;

	private void Start () 
	{
		MinimapManager.Instance.Minimap = this;
		UpdateMiniMap();
    }

	/// <summary>
	///  更新小地图信息
	/// </summary>
	public void UpdateMiniMap()
	{
        mapName.text = User.Instance.CurrentMapData.Name;
		miniMap.overrideSprite = MinimapManager.Instance.LoadCurrentMinimap();
        miniMap.SetNativeSize();
		miniMap.transform.localPosition = Vector3.zero;
		miniMapColliderBox = MinimapManager.Instance.MinimapBoundingBox;
		playerTF = null;
    }

	/// <summary>
	/// 小地图实现
	/// </summary>
	private void Update()
	{
        if (playerTF == null)
            playerTF = MinimapManager.Instance.PlayerTransform;

        if (miniMapColliderBox == null || playerTF == null) return;

		float width = miniMapColliderBox.bounds.size.x;
		float height = miniMapColliderBox.bounds.size.z;

		float playerX = playerTF.transform.position.x - miniMapColliderBox.bounds.min.x;
		float playerY = playerTF.transform.position.z - miniMapColliderBox.bounds.min.z;

		miniMap.rectTransform.pivot = new Vector2(playerX / width, playerY / height);

		arrow.rectTransform.eulerAngles = new Vector3(0, 0, -playerTF.transform.eulerAngles.y);
    }
}
