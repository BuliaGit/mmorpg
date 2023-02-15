using Managers;
using Models;
using UnityEngine;
using UnityEngine.UI;

public class UIMinimap : MonoBehaviour
{
    private Collider minimapBoundingBox;
    public Image minimap;
    public Image arrow;
    public Text mapName;

    /// <summary>
    /// 角色地图位置
    /// </summary>
    private Transform playerTransform;  //缓存单例获取的数据

    // Use this for initialization
    void Start()
    {
        //Debug.LogWarning("UIMinimap Start " + this.GetInstanceID());

        //MinimapManager.Instance.Minimap = this;
    }

    public void UpdateMap()
    {
        this.mapName.text = User.Instance.CurrentMapData.Name;
        this.minimap.overrideSprite = MinimapManager.Instance.LoadCurrentMinimap();
        this.minimap.SetNativeSize();
        this.minimap.transform.localPosition = Vector3.zero;
        this.minimapBoundingBox = MinimapManager.Instance.MinimapBoundingBox;
        this.playerTransform = null;
    }

    // Update is called once per frame
    void Update()
    {
        //方法一：
        if (playerTransform == null)
            playerTransform = MinimapManager.Instance.PlayerTransform;

        if (minimapBoundingBox == null || playerTransform == null) return;

        float realWidth = minimapBoundingBox.bounds.size.x;
        float realHeight = minimapBoundingBox.bounds.size.z;

        float relaX = playerTransform.position.x - minimapBoundingBox.bounds.min.x; //左下角
        float relaY = playerTransform.position.z - minimapBoundingBox.bounds.min.z;

        float pivotX = relaX / realWidth;
        float pivotY = relaY / realHeight;

        this.minimap.rectTransform.pivot = new Vector2(pivotX, pivotY);
        this.minimap.rectTransform.localPosition = Vector2.zero;
        this.arrow.transform.eulerAngles = new Vector3(0, 0, -playerTransform.eulerAngles.y);

        //方法二
        //SmallMapHelper.Instance.transform.position = playerTransform.transform.position;
        //this.minimap.transform.localPosition = new Vector3(SmallMapHelper.Instance.transform.localPosition.x * -685, SmallMapHelper.Instance.transform.localPosition.y * -654, 1);
        //this.arrow.transform.eulerAngles = new Vector3(0, 0, -playerTransform.eulerAngles.y);
    }
}
