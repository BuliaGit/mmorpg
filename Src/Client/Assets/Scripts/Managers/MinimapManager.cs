using Models;
using System.Collections;
using UnityEngine;

namespace Managers
{
    /// <summary>
    /// 小地图管理器
    /// </summary>
    class MinimapManager : Singleton<MinimapManager>
    {
        private UIMiniMapMe minimap;
        public UIMiniMapMe Minimap
        {
            get { return minimap; }
            set {
                minimap = value;
                Debug.LogWarningFormat("MinimapManager.Instance.Minimap[{0}] Set", minimap.GetInstanceID());
            }
        }

        private Collider minimapBoundingBox;
        public Collider MinimapBoundingBox
        {
            get { return minimapBoundingBox; }
        }

        public Transform PlayerTransform
        {
            get
            {
                if (User.Instance.CurrentCharacterObject == null)
                    return null;
                return User.Instance.CurrentCharacterObject.transform;
            }
        }

        /// <summary>
        /// 加载当前小地图数据
        /// </summary>
        /// <returns></returns>
        public Sprite LoadCurrentMinimap()
        {
            return Resloader.Load<Sprite>("UI/Minimap/" + User.Instance.CurrentMapData.MiniMap);
        }

        public void UpdateMinimap(Collider minimapBoundingBox)
        {
            this.minimapBoundingBox = minimapBoundingBox;
            if (this.Minimap != null)
            {
                this.Minimap.UpdateMiniMap();
            }
        }
    }
}
