using Assets.Scripts.Entities;
using SkillBridge.Message;
using UnityEngine;

namespace Entities
{
    /// <summary>
    /// 用于参与客户端实际逻辑的实体信息 
    /// </summary>
    public class Entity
    {
        public int entityId;

        public Vector3Int position;
        public Vector3Int direction;
        public int speed;

        public IEntityController Controller;

        private NEntity entityData;
        /// <summary>
        /// 用于网络通讯，同步服务器的实体信息_缓存器
        /// </summary>
        public NEntity EntityData
        {
            get {
                UpdateEntityData();
                return entityData;
            }
            set {
                entityData = value;
                this.SetEntityData(value);
            }
        }

        public Entity(NEntity entity)
        {
            this.entityId = entity.Id;
            this.entityData = entity;
            this.SetEntityData(entity);
        }

        public virtual void OnUpdate(float delta)
        {
            if (this.speed != 0)
            {
                Vector3 dir = this.direction;
                this.position += Vector3Int.RoundToInt(dir * speed * delta / 100f);
            }
        }

        //==================移动同步的关键（数据转换）=======================
        public void SetEntityData(NEntity entity)
        {
            this.position = this.position.FromNVector3(entity.Position);
            this.direction = this.direction.FromNVector3(entity.Direction);
            this.speed = entity.Speed;
        }

        public void UpdateEntityData()
        {
            entityData.Position.FromVector3Int(this.position);
            entityData.Direction.FromVector3Int(this.direction);
            entityData.Speed = this.speed;
        }
    }
}
