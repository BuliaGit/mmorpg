using Managers;
using Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Managers.Me
{
    class GuildManager : Singleton<GuildManager>
    {
        public NGuildInfo guildInfo;
        //自己当前公会成员信息
        public NGuildMemberInfo myMemberInfo;

        public bool HasGuild
        {
            get { return guildInfo != null; }
        }

        public void Init(NGuildInfo nGuildInfo)
        {
            guildInfo = nGuildInfo;
            if (nGuildInfo == null)
            {
                myMemberInfo = null;
                return;
            }
            foreach (var mem in nGuildInfo.Members)
            {
                if(mem.characterId == User.Instance.CurrentCharacterInfo.Id)
                {
                    myMemberInfo = mem;
                    break;
                }
            }
        }

        internal void ShowGuild()
        {
            if (HasGuild)
            {
                UIManagerMe.Instance.Show<UIGuild>();
            }
            else
            {
                UIManagerMe.Instance.Show<UIGuildPopNoGuild>().OnClose += OnClosePopNoGuild;
            }
        }

        private void OnClosePopNoGuild(UIWindowMe sender, UIWindowMe.WindowResult result)
        {
            if(result == UIWindowMe.WindowResult.Yes)//创建公会
            {
                UIManagerMe.Instance.Show<UIGuildPopCreate>();
            }
            else
            {
                if(result == UIWindowMe.WindowResult.No)//加入公会
                {
                    UIManagerMe.Instance.Show<UIGuildList>();
                }
            }
        }
    }
}
