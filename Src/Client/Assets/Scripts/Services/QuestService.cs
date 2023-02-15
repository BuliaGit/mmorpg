using Managers;
using Models;
using Network;
using SkillBridge.Message;
using System;
using UnityEngine;

namespace Services
{
    class QuestService : Singleton<QuestService>, IDisposable
    {
        public QuestService()
        {
            MessageDistributer.Instance.Subscribe<QuestAcceptResponse>(this.OnQuestAccept);
            MessageDistributer.Instance.Subscribe<QuestSubmitResponse>(this.OnQuestSubmit);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<QuestAcceptResponse>(this.OnQuestAccept);
            MessageDistributer.Instance.Unsubscribe<QuestSubmitResponse>(this.OnQuestSubmit);
        }

        #region 接受任务
        /// <summary>
        /// 发送接受任务请求
        /// </summary>
        /// <param name="quest"></param>
        /// <returns></returns>
        public bool SendQuestAccept(Quest quest)
        {
            Debug.Log("SendQuestAccept");

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.questAccept = new QuestAcceptRequest();
            message.Request.questAccept.QuestId = quest.Define.ID;

            NetClient.Instance.SendMessage(message);
            return true;
        }

        private void OnQuestAccept(object sender, QuestAcceptResponse message)
        {
            Debug.LogFormat("OnQuestAccept:{0},Err:{1}", message.Result, message.Errormsg);

            if (message.Result == Result.Success)
            {
                QuestManager.Instance.OnQuestAccepted(message.Quest);
            }
            else
            {
                MessageBox.Show("任务接受失败:", "错误", MessageBoxType.Error);
            }
        }
        #endregion

        #region 提交任务
        /// <summary>
        /// 发送提交任务
        /// </summary>
        /// <param name="quest"></param>
        /// <returns></returns>
        public bool SendQuestSubmit(Quest quest)
        {
            Debug.Log("SendQuestSubmit");

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.questSubmit = new QuestSubmitRequest();
            message.Request.questSubmit.QuestId = quest.Define.ID;

            NetClient.Instance.SendMessage(message);
            return true;
        }

        private void OnQuestSubmit(object sender, QuestSubmitResponse message)
        {
            Debug.LogFormat("OnQuestSubmit:{0},Err:{1}", message.Result, message.Errormsg);

            if (message.Result == Result.Success)
            {
                QuestManager.Instance.OnQuestSubmited(message.Quest);
            }
            else
            {
                MessageBox.Show("完成任务失败:", "错误", MessageBoxType.Error);
            }
        }
        #endregion
    }
}
