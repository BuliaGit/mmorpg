﻿using GameServer.Core;
using SkillBridge.Message;

namespace GameServer.Entities
{
    /// <summary>
    /// 实体信息【游戏中所有（看得见摸得着的）对象】
    /// </summary>
    class Entity
    {
        public int entityId
        {
            get { return this.entityData.Id; }
        }


        private Vector3Int position;

        public Vector3Int Position
        {
            get { return position; }
            set 
            {
                position = value;
                this.entityData.Position = position;
            }
        }

        private Vector3Int direction;
        public Vector3Int Direction
        {
            get { return direction; }
            set
            {
                direction = value;
                this.entityData.Direction = direction;
            }
        }

        private int speed;
        public int Speed
        {
            get { return speed; }
            set
            {
                speed = value;
                this.entityData.Speed = speed;
            }
        }

        private NEntity entityData;
        public NEntity EntityData
        {
            get
            {
                return entityData;
            }
            set
            {
                entityData = value;
                this.SetEntityData(value);
            }
        }

        public Entity(Vector3Int pos,Vector3Int dir)
        {
            this.entityData = new NEntity();
            this.entityData.Position = pos;
            this.entityData.Direction = dir;
            this.SetEntityData(this.entityData);
        }

        public Entity(NEntity entity)
        {
            this.entityData = entity;
        }

        public void SetEntityData(NEntity entity)
        {
            this.Position = entity.Position;
            this.Direction = entity.Direction;
            this.speed = entity.Speed;
        }

        public virtual void Update()
        {

        }
    }
}
