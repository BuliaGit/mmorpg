using Common.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class NpcManagerMe : Singleton<NpcManagerMe>
    {
        public delegate bool NpcActionHandler(NpcDefine npcDefine);

        Dictionary<NpcFunction, NpcActionHandler> eventDic = new Dictionary<NpcFunction, NpcActionHandler>();
        Dictionary<int,Vector3> npcPositions = new Dictionary<int, Vector3>();

        public void RegisterNpcEvent(NpcFunction function, NpcActionHandler action)
        {
            if (!eventDic.ContainsKey(function))
            {
                eventDic[function] = action;
            }
            else
            {
                eventDic[function] += action;
            }
        }


        public NpcDefine GetNpcDefine(int npcID)
        {
            NpcDefine npcDefine = null;
            DataManager.Instance.Npcs.TryGetValue(npcID, out npcDefine);
            return npcDefine;
        }

        /// <summary>
        /// 进行互动
        /// </summary>
        /// <param name="npcID"></param>
        /// <returns></returns>
        public bool Interactive(int npcID)
        {
            if (DataManager.Instance.Npcs.ContainsKey(npcID))
            {
                var npc = DataManager.Instance.Npcs[npcID];
                return Interactive(npc);
            }
            return false;
        }

        /// <summary>
        /// 分类型互动
        /// </summary>
        /// <param name="npc"></param>
        /// <returns></returns>
        public bool Interactive(NpcDefine npc)
        {
            if (DoTaskInteractive(npc))
            {
                return true;
            }
            else
            {
                if(npc.Type == NpcType.Functional)
                {
                    DoFunctionInteractive(npc);
                }
            }
            return false;
        }

        private bool DoTaskInteractive(NpcDefine npc)
        {
            //通过npcID获取其任务状态
            var status = QuestManager.Instance.GetQuestStatusByNpc(npc.ID);
            if (status == NpcQuestStatus.None)
                return false;

            return QuestManager.Instance.OpenNpcQuest(npc.ID);
        }

        private bool DoFunctionInteractive(NpcDefine npc)
        {
            if (!eventDic.ContainsKey(npc.Function))
            {
                return false;
            }
            return eventDic[npc.Function](npc);
        }

        public void UpdateNpcPosition(int npc,Vector3 pos)
        {
            npcPositions[npc] = pos;
        }
        internal Vector3 GetNpcPosition(int npc)
        {
            return npcPositions[npc];
        }
    }
}