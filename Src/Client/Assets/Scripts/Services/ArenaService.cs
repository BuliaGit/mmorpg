using Assets.Scripts.Managers;
using Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Experimental.AI;

namespace Assets.Scripts.Services
{
    class ArenaService : Singleton<ArenaService>
    {
        public void Init()
        {

        }

        public ArenaService()
        {
            MessageDistributer.Instance.Subscribe<ArenaChallengeRequest>(OnArenaChallengeRequest); //别人的组队请求
            MessageDistributer.Instance.Subscribe<ArenaChallengeResponse>(OnArenaChallengeResponse);
            MessageDistributer.Instance.Subscribe<ArenaBeginResponse>(OnArenaBegin);
            MessageDistributer.Instance.Subscribe<ArenaEndResponse>(OnArenaEnd);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<ArenaChallengeRequest>(OnArenaChallengeRequest);
            MessageDistributer.Instance.Unsubscribe<ArenaChallengeResponse>(OnArenaChallengeResponse);
            MessageDistributer.Instance.Unsubscribe<ArenaBeginResponse>(OnArenaBegin);
            MessageDistributer.Instance.Unsubscribe<ArenaEndResponse>(OnArenaEnd);
        }
        public void SendArenaChangeRequest(int friendId, string friendName)
        {
            Debug.Log("SendArenaChangeRequest");
            NetMessage msg = new NetMessage();
            msg.Request = new NetMessageRequest();
            msg.Request.arenaChallengeReq = new ArenaChallengeRequest()
            {
                ArenaInfo = new ArenaInfo()
                {
                    Red = new ArenaPlayer()
                    {
                        EntityId = User.Instance.CurrentCharacterInfo.Id,
                        Name = User.Instance.CurrentCharacterInfo.Name
                    },
                    Blue = new ArenaPlayer()
                    {
                        EntityId = friendId,
                        Name = friendName
                    }
                }
            };
            NetClient.Instance.SendMessage(msg);
        }

        public void SendArenaChallengeResponse(bool accept,ArenaChallengeRequest request)
        {
            Debug.Log("SendArenaChallengeResponse");
            NetMessage msg = new NetMessage();
            msg.Request = new NetMessageRequest();
            msg.Request.arenaChallengeRes = new ArenaChallengeResponse()
            {
                Result = accept ? Result.Success : Result.Failed,
                Errormsg = accept ? "" : "对方拒绝了挑战请求",
                ArenaInfo = request.ArenaInfo
            };
            NetClient.Instance.SendMessage(msg);
        }

        private void OnArenaChallengeRequest(object sender, ArenaChallengeRequest message)
        {
            throw new NotImplementedException();
        }

        private void OnArenaChallengeResponse(object sender, ArenaChallengeResponse message)
        {
            Debug.Log("OnArenaChallengeResponse");
            if(message.Result != Result.Success)
            {
                MessageBox.Show(message.Errormsg, "对方拒绝挑战");
            }
        }

        private void OnArenaBegin(object sender, ArenaBeginResponse message)
        {
            Debug.Log("OnArenaBegin");
            ArenaManager.Instance.EnterArena(message.ArenaInfo);
        }

        private void OnArenaEnd(object sender, ArenaEndResponse message)
        {
            Debug.Log("OnArenaEnd");
            ArenaManager.Instance.ExitArena(message.ArenaInfo);
        }



    }
}
