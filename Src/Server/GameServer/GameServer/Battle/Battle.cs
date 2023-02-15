using GameServer.Core;
using GameServer.Entities;
using GameServer.Managers;
using GameServer.Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Battle
{
    class Battle
    {
        public Map Map;

        /// <summary>
        /// 所有参加战斗的单位
        /// </summary>
        Dictionary<int, Creature> AllUnits = new Dictionary<int, Creature>();

        Queue<NSkillCastInfo> Actions = new Queue<NSkillCastInfo>();

        List<Creature> DeathPool = new List<Creature>();

        List<NSkillHitInfo> Hits = new List<NSkillHitInfo>();

        List<NSkillCastInfo> CastSkills = new List<NSkillCastInfo>();

        List<NBuffInfo> BuffActions = new List<NBuffInfo>();

        public Battle(Map map)
        {
            Map = map;
        }

        public void ProcessBattleMessage(NetConnection<NetSession> sender, SkillCastRequest request)
        {
            Character character = sender.Session.Character;
            if (request.castInfo != null)
            {
                if (character.entityId != request.castInfo.casterId)
                {
                    return;
                }
                Actions.Enqueue(request.castInfo);
            }
        }

        public void Update()
        {
            CastSkills.Clear();
            Hits.Clear();
            BuffActions.Clear();
            if (Actions.Count > 0)
            {
                NSkillCastInfo skillCast = Actions.Dequeue();
                ExecuteAction(skillCast);
            }
            UpdateUnits();

            BroadCastHitsMessage();
        }

        private void BroadCastHitsMessage()
        {
            if (Hits.Count == 0 && BuffActions.Count == 0 && CastSkills.Count == 0) return;
            NetMessageResponse msg = new NetMessageResponse();

            if(CastSkills.Count > 0)
            {
                msg.skillCast = new SkillCastResponse();
                msg.skillCast.castInfoes.AddRange(CastSkills);
                msg.skillCast.Result = Result.Success;
                msg.skillCast.Errormsg = "";
            }
            if (Hits.Count > 0)
            {
                msg.skillHits = new SkillHitResponse();
                msg.skillHits.Hits.AddRange(Hits);
                msg.skillHits.Result = Result.Success;
                msg.skillHits.Errormsg = "";
            }
            if (BuffActions.Count > 0)
            {
                msg.buffRes = new BuffResponse();
                msg.buffRes.Buffs.AddRange(BuffActions);
                msg.buffRes.Result = Result.Success;
                msg.buffRes.Errormsg = "";
            }
            Map.BroadCastBattleResponse(msg);
        }

        public void JoinBattle(Creature unit)
        {
            AllUnits[unit.entityId] = unit;
        }

        public void LeaveBattle(Creature unit)
        {
            AllUnits.Remove(unit.entityId);
        }

        private void UpdateUnits()
        {
            DeathPool.Clear();
            foreach (var item in AllUnits)
            {
                item.Value.Update();
                if (item.Value.IsDeath)
                {
                    DeathPool.Add(item.Value);
                }
            }

            foreach (var item in DeathPool)
            {
                LeaveBattle(item);
            }
        }

        private void ExecuteAction(NSkillCastInfo skillCast)
        {
            BattleContext context = new BattleContext(this);
            context.Caster = EntityManager.Instance.GetCreature(skillCast.casterId);
            context.Target = EntityManager.Instance.GetCreature(skillCast.targetId);
            context.CastSkill = skillCast;

            //context.Position = skillCast.Position;

            if (context.Caster != null)
                JoinBattle(context.Caster);
            if (context.Target != null)
                JoinBattle(context.Target);

            context.Caster.CastSkill(context, skillCast.skillId);
        }

        internal List<Creature> FindUnitsInRange(Vector3Int pos, int aoeRange)
        {
            List<Creature> result = new List<Creature>();
            foreach (var unit in AllUnits)
            {
                if (unit.Value.Distance(pos) < aoeRange)
                {
                    result.Add(unit.Value);
                }
            }
            return result;
        }

        internal List<Creature> FindUnitsInMapRange(Vector3Int pos, int range)
        {
            return EntityManager.Instance.GetMapEntitiesInRange<Creature>(Map.ID, pos, range);
        }

        public void AddCastSkillInfo(NSkillCastInfo cast)
        {
            CastSkills.Add(cast);
        }

        public void AddHitInfo(NSkillHitInfo hitInfo)
        {
            Hits.Add(hitInfo);
        }

        internal void AddBuffAction(NBuffInfo buffInfo)
        {
            BuffActions.Add(buffInfo);
        }
    }
}
