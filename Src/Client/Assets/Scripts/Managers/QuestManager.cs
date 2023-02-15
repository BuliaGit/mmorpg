using Models;
using Services;
using SkillBridge.Message;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

namespace Managers
{
    public enum NpcQuestStatus
    {
        None = 0,//无任务
        Complete,//拥有已完成可提交任务
        Available,//拥有可接受任务
        Incomolete,//拥有未完成任务
    }

    /// <summary>
    /// 任务管理器
    /// </summary>
    class QuestManager : Singleton<QuestManager>
    {
        /// <summary>
        /// 网络任务信息列表 
        /// </summary>
        public List<NQuestInfo> questInfos;

        /// <summary>
        /// 所有任务
        /// </summary>
        public Dictionary<int, Quest> allQuests = new Dictionary<int, Quest>();

        /// <summary>
        /// NPC任务字典[NPC的Id：任务列表状态]
        /// </summary>
        public Dictionary<int, Dictionary<NpcQuestStatus, List<Quest>>> npcQuests = new Dictionary<int, Dictionary<NpcQuestStatus, List<Quest>>>();

        public UnityAction<Quest> onQuestStatusChanged;

        #region 初始化
        public void Init(List<NQuestInfo> quests)
        {
            questInfos = quests;

            allQuests.Clear();
            npcQuests.Clear();

            InitQuests();
        }

        private void InitQuests()
        {
            //1、初始化已接的任务
            foreach (var info in this.questInfos)
            {
                Quest quest = new Quest(info);
                allQuests[quest.Info.QuestId] = quest;
            }

            //2、初始化可接的任务
            CheckAvailableQuests();

            //3、让每个NPC添加任务状态
            foreach (var kv in this.allQuests)
            {
                AddNpcQuest(kv.Value.Define.AcceptNpc, kv.Value);
                AddNpcQuest(kv.Value.Define.SubmitNpc, kv.Value);
            }
        }

        /// <summary>
        /// 检查并添加可接的任务
        /// </summary>
        void CheckAvailableQuests()
        {
            foreach (var kv in DataManager.Instance.Quests)
            {
                //验证
                if (kv.Value.LimitClass != CharacterClass.None && kv.Value.LimitClass != User.Instance.CurrentCharacterInfo.Class)
                    continue;//不符合职业

                if (kv.Value.LimitLevel > User.Instance.CurrentCharacterInfo.Level)
                    continue;//不符合等级

                if (allQuests.ContainsKey(kv.Key))
                    continue;//任务已存在

                if (kv.Value.PreQuest > 0)
                {
                    Quest preQuest;
                    if (allQuests.TryGetValue(kv.Value.PreQuest, out preQuest))//获取前置任务
                    {
                        if (preQuest.Info == null)
                            continue;//前置任务未接取
                        if (preQuest.Info.Status != QuestStatus.Finished)
                            continue;//前置任务未完成
                    }
                    else
                        continue;//前置任务未完成
                }

                //添加任务
                Quest quest = new Quest(kv.Value);
                allQuests[quest.Define.ID] = quest;
            }
        }
        #endregion

        #region AddNpcQuest 添加NPC任务状态
        /// <summary>
        /// 添加NPC任务状态
        /// </summary>
        /// <param name="npcId"></param>
        /// <param name="quest"></param>
        private void AddNpcQuest(int npcId, Quest quest)
        {
            if (!npcQuests.ContainsKey(npcId))
                npcQuests[npcId] = new Dictionary<NpcQuestStatus, List<Quest>>();
            //初始化
            List<Quest> availables;
            List<Quest> complates;
            List<Quest> incomplates;

            if (!npcQuests[npcId].TryGetValue(NpcQuestStatus.Available, out availables))
            {
                availables = new List<Quest>();
                npcQuests[npcId][NpcQuestStatus.Available] = availables;
            }
            if (!npcQuests[npcId].TryGetValue(NpcQuestStatus.Complete, out complates))
            {
                complates = new List<Quest>();
                npcQuests[npcId][NpcQuestStatus.Complete] = complates;
            }
            if (!npcQuests[npcId].TryGetValue(NpcQuestStatus.Incomolete, out incomplates))
            {
                incomplates = new List<Quest>();
                npcQuests[npcId][NpcQuestStatus.Incomolete] = incomplates;
            }

            if (quest.Info == null)
            {
                //可接
                if (npcId == quest.Define.AcceptNpc && !npcQuests[npcId][NpcQuestStatus.Available].Contains(quest))
                {
                    npcQuests[npcId][NpcQuestStatus.Available].Add(quest);
                }
            }
            else
            {   
                //已完成
                if (quest.Define.SubmitNpc == npcId && quest.Info.Status == QuestStatus.Complated)
                {
                    if (!npcQuests[npcId][NpcQuestStatus.Complete].Contains(quest))
                    {
                        npcQuests[npcId][NpcQuestStatus.Complete].Add(quest);
                    }
                }
                //进行中
                if (quest.Define.SubmitNpc == npcId && quest.Info.Status == QuestStatus.InProgress)
                {
                    if (!npcQuests[npcId][NpcQuestStatus.Incomolete].Contains(quest))
                    {
                        npcQuests[npcId][NpcQuestStatus.Incomolete].Add(quest);
                    }
                }
            }
        }
        #endregion

        #region GetQuestStatusByNpc 获取NPC任务状态
        /// <summary>
        /// 获取NPC任务状态
        /// </summary>
        /// <param name="npcId"></param>
        /// <returns></returns>
        public NpcQuestStatus GetQuestStatusByNpc(int npcId)
        {
            Dictionary<NpcQuestStatus, List<Quest>> status = new Dictionary<NpcQuestStatus, List<Quest>>();
            if (npcQuests.TryGetValue(npcId, out status))
            {
                if (status[NpcQuestStatus.Complete].Count > 0)
                    return NpcQuestStatus.Complete;
                if (status[NpcQuestStatus.Available].Count > 0)
                    return NpcQuestStatus.Available;
                if (status[NpcQuestStatus.Incomolete].Count > 0)
                    return NpcQuestStatus.Incomolete;
            }
            return NpcQuestStatus.None;
        }
        #endregion

        #region 任务对话框
        /// <summary>
        /// 打开任务对话框
        /// </summary>
        /// <param name="npcId"></param>
        /// <returns></returns>
        public bool OpenNpcQuest(int npcId)
        {
            Dictionary<NpcQuestStatus, List<Quest>> status = new Dictionary<NpcQuestStatus, List<Quest>>();

            if (npcQuests.TryGetValue(npcId, out status))  //获取NPC任务
            {
                //拥有已完成可提交任务
                if (status[NpcQuestStatus.Complete].Count > 0)
                    return ShowQuestDialog(status[NpcQuestStatus.Complete].First());
                //拥有可接受任务
                if (status[NpcQuestStatus.Available].Count > 0)
                    return ShowQuestDialog(status[NpcQuestStatus.Available].First());
                //拥有未完成任务
                if (status[NpcQuestStatus.Incomolete].Count > 0)
                    return ShowQuestDialog(status[NpcQuestStatus.Incomolete].First());
            }
            return false;
        }

        /// <summary>
        /// 显示任务对话框
        /// </summary>
        /// <param name="quest"></param>
        /// <returns></returns>
        private bool ShowQuestDialog(Quest quest)
        {

            if (quest.Info == null || quest.Info.Status == QuestStatus.Complated)
            {
                UIQuestDialog dlg = UIManagerMe.Instance.Show<UIQuestDialog>();
                dlg.SetQuest(quest);
                dlg.OnClose += OnQuestDialogClose;
                return true;
            }
            //任务未完成
            if (quest.Info != null || quest.Info.Status == QuestStatus.Complated)
            {
                if (!string.IsNullOrEmpty(quest.Define.DialogIncomplete))
                    MessageBox.Show(quest.Define.DialogIncomplete);
            }
            return true;
        }


        private void OnQuestDialogClose(UIWindowMe sender, UIWindowMe.WindowResult result)
        {
            UIQuestDialog dlg = (UIQuestDialog)sender;
            if (result == UIWindowMe.WindowResult.Yes)
            {
                if (dlg.quest.Info == null)
                    QuestService.Instance.SendQuestAccept(dlg.quest);//任务接受请求
                else if (dlg.quest.Info.Status == QuestStatus.Complated)
                    QuestService.Instance.SendQuestSubmit(dlg.quest);//任务完成请求
            }
            else if (result == UIWindowMe.WindowResult.No)
            {
                MessageBox.Show(dlg.quest.Define.DialogDeny);
            }
        }
        #endregion

        #region 任务反馈提示
        /// <summary>
        /// 接受任务反馈
        /// </summary>
        /// <param name="info"></param>
        public void OnQuestAccepted(NQuestInfo info)
        {
            var quest = RefreshQuestStatus(info);
            MessageBox.Show(quest.Define.DialogAccept);
        }

        /// <summary>
        /// 提交任务反馈
        /// </summary>
        /// <param name="info"></param>
        public void OnQuestSubmited(NQuestInfo info)
        {
            var quest = RefreshQuestStatus(info);
            MessageBox.Show(quest.Define.DialogFinish);
        }

        /// <summary>
        /// 刷新任务状态
        /// </summary>
        /// <param name="quest"></param>
        /// <returns></returns>
        Quest RefreshQuestStatus(NQuestInfo quest)
        {
            //1、清空
            npcQuests.Clear();

            //2、同步
            Quest result;
            if (allQuests.ContainsKey(quest.QuestId))
            {
                //更新新的任务状态
                allQuests[quest.QuestId].Info = quest;
                result = allQuests[quest.QuestId];
            }
            else
            {
                result = new Quest(quest);
                allQuests[quest.QuestId] = result;
            }


            CheckAvailableQuests();

            foreach (var kv in allQuests)
            {
                //分别给任务接取和完成的NPC赋值任务
                AddNpcQuest(kv.Value.Define.AcceptNpc, kv.Value);
                AddNpcQuest(kv.Value.Define.SubmitNpc, kv.Value);
            }

            if (onQuestStatusChanged != null)
                onQuestStatusChanged(result);

            return result;
        }
        #endregion
    }
}
