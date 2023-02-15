using Common.Data;
using Entities;
using SkillBridge.Message;

namespace Models
{
    /// <summary>
    /// 用户类
    /// </summary>
    class User : Singleton<User>
    {
        #region 缓存服务器的数据

        NUserInfo userInfo;

        //网络用户信息
        public NUserInfo Info { get { return userInfo; } }
        //队伍信息
        public NTeamInfo TeamInfo { get; set; }
        public NCharacterInfo CurrentCharacterInfo { get; set; }

        public Creature CurrentCharacter { get; set; } 

        #endregion

        #region 本地
        public MapDefine CurrentMapData { get; set; }

        public PlayerInputController CurrentCharacterObject { get; set; }
        #endregion


        /// <summary>
        /// 缓存服务器的用户信息，而服务器的用户信息来自数据库
        /// </summary>
        /// <param name="info">实际是角色信息</param>
        public void SetupUserInfo(NUserInfo info)
        {
            this.userInfo = info;
        }

        public void AddGold(int gold)
        {
            this.CurrentCharacterInfo.Gold += gold;
        }

        public int CurrentRide = 0;
        public void Ride(int id)
        {
            if(CurrentRide != id)
            {
                CurrentRide = id;
                CurrentCharacterObject.SendEntityEvent(EntityEvent.Ride,CurrentRide);
            }
            else
            {
                CurrentRide = 0;
                CurrentCharacterObject.SendEntityEvent(EntityEvent.Ride, 0);
            }
        }

        public delegate void CharacterInitHandler();
        public event CharacterInitHandler OnCharacterInit;

        internal void CharacterInited()
        {
            if (OnCharacterInit != null)
            {
                OnCharacterInit();
            }
        }
    }
}
