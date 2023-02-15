using Common.Battle;
using GameServer.AI;
using GameServer.Battle;
using GameServer.Core;
using GameServer.Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Entities
{
    class Monster : Creature
    {
        public Map Map;

        AIAgent AI;
        private Vector3Int moveTarget;

        Vector3 movePosition;

        public Monster(int tid, int level, Vector3Int pos, Vector3Int dir) : base(CharacterType.Monster, tid, level, pos, dir)
        {
            this.AI = new AIAgent(this);
        }


        public void OnEnterMap(Map map)
        {
            this.Map = map;
        }

        public override void Update()
        {
            base.Update();
            UpdateMovement();
            this.AI.Update();
        }



        public Skill FindSkill(BattleContext context, SkillType type)
        {
            Skill cancast = null;
            foreach (var skill in SkillMgr.Skills)
            {
                if ((skill.Define.Type & type) != skill.Define.Type) continue;

                var result = skill.CanCast(context);
                if (result == SkillResult.Casting)
                {
                    return null;
                }
                if (result == SkillResult.Ok)
                {
                    cancast = skill;
                }
            }
            return cancast;
        }

        public override void OnDamage(NDamageInfo damage, Creature source)
        {
            if (AI != null)
            {
                AI.OnDamage(damage, source);
            }
        }

        internal void StopMove()
        {
            State = CharacterState.Idle;
            moveTarget = Vector3Int.zero;
            Speed = 0;

            NEntitySync sync = new NEntitySync()
            {
                Entity = EntityData,
                Event = EntityEvent.Idle,
                Id = entityId
            };
            Map.UpdateEntity(sync);
        }

        internal void MoveTo(Vector3Int position)
        {
            if (State == CharacterState.Idle)
            {
                State = CharacterState.Move;
            }
            if (moveTarget != position)
            {
                moveTarget = position;
                movePosition = Position;//

                var dist = (moveTarget - Position);//

                Direction = dist.normalized;

                Speed = Define.Speed;

                NEntitySync sync = new NEntitySync()
                {
                    Entity = EntityData,
                    Event = EntityEvent.MoveFwd,
                    Id = entityId
                };
                Map.UpdateEntity(sync);
            }
        }
        private void UpdateMovement()
        {
            if (State == CharacterState.Move)
            {
                if (Distance(moveTarget) < 50)
                {
                    StopMove();
                }
                if (Speed > 0)
                {
                    Vector3 dir = Direction;
                    movePosition += dir * Speed * Time.deltaTime / 100f;
                    Position = movePosition;
                }
            }
        }
    }
}
