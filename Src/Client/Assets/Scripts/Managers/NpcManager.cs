using Common.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    /// <summary>
    /// Npc管理器
    /// </summary>
    public class NpcManager : Singleton<NpcManager>
    {
        public delegate bool NpcActionHandler(NpcDefine npc);

        /// <summary>
        ///NPC行为事件字典【NPC类型：NPC行为】
        /// </summary>
        Dictionary<NpcFunction, NpcActionHandler> EventMap = new Dictionary<NpcFunction, NpcActionHandler>();
        //Dictionary<int, Vector3> npcPostions = new Dictionary<int, Vector3>();

        /// <summary>
        /// 注册NPC行为事件
        /// </summary>
        /// <param name="function"></param>
        /// <param name="action"></param>
        public void RegisterNpcEvent(NpcFunction function, NpcActionHandler action)
        {
            if (!EventMap.ContainsKey(function))
                EventMap[function] = action;
            else
                EventMap[function] += action;
        }

        /// <summary>
        /// 获取某个NPC配置信息
        /// </summary>
        /// <param name="npcID"></param>
        /// <returns></returns>
        public NpcDefine GetNpcDefine(int npcID)
        {
            return DataManager.Instance.Npcs[npcID];
        }

        #region 互动
        public bool Interactive(int npcId)
        {
            if (DataManager.Instance.Npcs.ContainsKey(npcId))
            {
                var npc = DataManager.Instance.Npcs[npcId];
                return Interactive(npc);
            }
            return false;
        }

        /// <summary>
        /// 是否互动
        /// </summary>
        /// <param name="npc"></param>
        /// <returns></returns>
        public bool Interactive(NpcDefine npc)
        {
            if (DoTaskInteractive(npc))
            {
                return true;
            }
            else if (npc.Type == NpcType.Functional)
            {
                return DoFunctionInteractive(npc);
            }
            return false;  
        }

        /// <summary>
        /// 任务互动
        /// </summary>
        /// <param name="npc"></param>
        /// <returns></returns>
        private bool DoTaskInteractive(NpcDefine npc)
        {
            var status = QuestManager.Instance.GetQuestStatusByNpc(npc.ID);
            if (status == NpcQuestStatus.None)
                return false;

            return QuestManager.Instance.OpenNpcQuest(npc.ID);
        }

        /// <summary>
        /// 功能互动
        /// </summary>
        /// <param name="npc"></param>
        /// <returns></returns>
        private bool DoFunctionInteractive(NpcDefine npc)
        {
            if (!EventMap.ContainsKey(npc.Function))
                return false;

            return EventMap[npc.Function](npc);
        }
        #endregion


        //public void UpdateNpcPostion(int npc,Vector3 pos)
        //{
        //    this.npcPostions[npc] = pos;
        //}
        //internal Vector3 GetNpcPostion(int npc)
        //{
        //    return this.npcPostions[npc];
        //}
    }
}
