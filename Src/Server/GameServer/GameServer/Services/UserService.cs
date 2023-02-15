using Common;
using GameServer.Entities;
using GameServer.Managers;
using GameServer.Models;
using Network;
using SkillBridge.Message;
using System.Linq;

namespace GameServer.Services
{
    class UserService : Singleton<UserService>
    {
        private TCharacter dbchar;

        public UserService()
        {
            //订阅客户端请求事件（保证收到消息）
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserLoginRequest>(this.OnLogin);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserRegisterRequest>(this.OnRegister);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserCreateCharacterRequest>(this.OnCreateCharacter);

            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserGameEnterRequest>(this.OnGameEnter);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserGameLeaveRequest>(this.OnGameLeave);
        }

        public void Init()
        {

        }

        #region 用户注册
        void OnRegister(NetConnection<NetSession> sender, UserRegisterRequest request)
        {
            Log.InfoFormat("UserRegisterRequest: User:{0}  Pass:{1}", request.User, request.Passward);

            sender.Session.Response.userRegister = new UserRegisterResponse();

            //查询该用户名
            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            if (user != null)
            {
                sender.Session.Response.userRegister.Result = Result.Success;
                sender.Session.Response.userRegister.Errormsg = "用户已存在.";
            }
            else
            {
                TPlayer player = DBService.Instance.Entities.Players.Add(new TPlayer());
                DBService.Instance.Entities.Users.Add(new TUser() { Username = request.User, Password = request.Passward, Player = player });
                DBService.Instance.Entities.SaveChanges();
                sender.Session.Response.userRegister.Result = Result.Success;
                sender.Session.Response.userRegister.Errormsg = "None";
            }

            sender.SendResponse();
        }
        #endregion

        #region 用户登录
        void OnLogin(NetConnection<NetSession> sender, UserLoginRequest request)
        {
            Log.InfoFormat("UserLoginRequest: User:{0}  Pass:{1}", request.User, request.Passward);


            sender.Session.Response.userLogin = new UserLoginResponse();

            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            if (user == null)
            {
                sender.Session.Response.userLogin.Result = Result.Failed;
                sender.Session.Response.userLogin.Errormsg = "用户不存在";
            }
            else if (user.Password != request.Passward)
            {
                sender.Session.Response.userLogin.Result = Result.Failed;
                sender.Session.Response.userLogin.Errormsg = "密码错误";
            }
            else
            {
                sender.Session.User = user; //todo : 重要 缓存数据库 用户信息 

                sender.Session.Response.userLogin.Result = Result.Success;
                sender.Session.Response.userLogin.Errormsg = "None";
                sender.Session.Response.userLogin.Userinfo = new NUserInfo();
                sender.Session.Response.userLogin.Userinfo.Id = (int)user.ID;   //DB的Id
                sender.Session.Response.userLogin.Userinfo.Player = new NPlayerInfo();
                sender.Session.Response.userLogin.Userinfo.Player.Id = user.Player.ID;
                foreach (var c in user.Player.Characters)
                {
                    NCharacterInfo info = new NCharacterInfo
                    {
                        Id = c.ID,  //DB的Id
                        Name = c.Name,
                        Type = CharacterType.Player,
                        Class = (CharacterClass)c.Class,
                        ConfigId = c.ID,
                        Level = c.Level
                    };
                    sender.Session.Response.userLogin.Userinfo.Player.Characters.Add(info);
                }
            }

            sender.SendResponse();
        }
        #endregion

        #region 角色创建
        void OnCreateCharacter(NetConnection<NetSession> sender, UserCreateCharacterRequest request)
        {
            Log.InfoFormat("UserCreateCharacterRequest: Name:{0}  Class:{1}", request.Name, request.Class);

            #region 数据库
            TCharacter character = new TCharacter()
            {
                Name = request.Name,
                Class = (int)request.Class,
                TID = (int)request.Class,
                Level = 1,
                MapID = 1,      //初始地图
                MapPosX = 5000,	//初始出生位置  相当于U3D坐标（50,8.2,40）
                MapPosY = 4000,
                MapPosZ = 820,
                MapDirY = 9000,
                Gold = 100000,  //初始10万金币
                HP = 1000,
                MP = 1000,
                Equips = new byte[28]
            };
            character = DBService.Instance.Entities.Characters.Add(character);

            TCharacterBag bag = new TCharacterBag
            {
                Owner = character,
                Items = new byte[0],
                Unlocked = 20  //初始格子数
            };
            character.Bag = DBService.Instance.Entities.CharacterBags.Add(bag);

            //新手大礼包
            character.Items.Add(new TCharacterItem()
            {
                Owner = character,
                ItemID = 1,
                ItemCount = 20,
            });
            character.Items.Add(new TCharacterItem()
            {
                Owner = character,
                ItemID = 2,
                ItemCount = 20,
            });

            sender.Session.User.Player.Characters.Add(character);   //todo：相当于缓存数据库该玩家的角色列表信息
            DBService.Instance.Save();
            #endregion

            #region 协议

            sender.Session.Response.createChar = new UserCreateCharacterResponse()
            {
                Result = Result.Success,
                Errormsg = "None"
            };

            foreach (var c in sender.Session.User.Player.Characters) //数据库
            {
                NCharacterInfo info = new NCharacterInfo
                {
                    Id = c.ID, //DB的Id
                    Name = c.Name,
                    Type = CharacterType.Player,
                    Class = (CharacterClass)c.Class,
                    ConfigId = c.TID,  //DB的Id
                    Level = c.Level
                };
                sender.Session.Response.createChar.Characters.Add(info);
            }

            sender.SendResponse();
            #endregion
        }
        #endregion

        #region 进入游戏
        void OnGameEnter(NetConnection<NetSession> sender, UserGameEnterRequest request)
        {
            //获取当前选择的角色在数据库的实体信息（来源于创建角色时的默认数据)
            dbchar = sender.Session.User.Player.Characters.ElementAt(request.characterIdx);
            Log.InfoFormat("UserGameEnterRequest: characterID:{0}:{1} Map:{2}", dbchar.ID, dbchar.Name, dbchar.MapID);

            Character character = CharacterManager.Instance.AddCharacter(dbchar);
            SessionManager.Instance.AddSession(character.Id, sender);

            sender.Session.Response.gameEnter = new UserGameEnterResponse()
            {
                Result = Result.Success,
                Errormsg = "None",
                Character = character.Info
            };

            sender.Session.Character = character;   //todo：重要 缓存数据库当前角色实体
            //后处理赋值
            sender.Session.PostResponser = character;

            //进入游戏成功，发送初始角色信息
            sender.SendResponse();
            MapManager.Instance[dbchar.MapID].CharacterEnter(sender, character);//MapManager.Instance.Maps[] 发送进入地图消息
        }
        #endregion

        #region 离开游戏
        /// <summary>
        /// 离开游戏
        /// </summary>
        /// <param name="sender">客户端的缓存器</param>
        /// <param name="request"></param>
        void OnGameLeave(NetConnection<NetSession> sender, UserGameLeaveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("UserGameLeaveRequest: characterID:{0}:{1} Map:{2}", character.Id, character.Info.Name, character.Info.mapId);
            SessionManager.Instance.RemoveSession(character.Id);
            CharacterLeave(character);

            sender.Session.Response.gameLeave = new UserGameLeaveResponse()
            {
                Result = Result.Success,
                Errormsg = "None"
            };

            sender.SendResponse();
        }

        public void CharacterLeave(Character character)
        {
            Log.InfoFormat("CharacterLeave： characterID:{0}:{1}", character.Id, character.Info.Name);
            
            ////记录角色上次离开游戏的地图和位置
            //dbchar.MapID = character.Info.mapId;
            //if (dbchar.MapID == 1)
            //{
            //    var curRole = MapManager.Instance[dbchar.MapID].curChar;
            //    dbchar.MapPosX = curRole.Position.x;
            //    dbchar.MapPosY = curRole.Position.y;
            //    dbchar.MapPosZ = curRole.Position.z;
            //    //dbchar.MapRotY = curRole.Direction.y *100;
            //}
            //else
            //{
            //    dbchar.MapID = 1;
            //    dbchar.MapPosX = MapService.Instance.back.Position.X;
            //    dbchar.MapPosY = MapService.Instance.back.Position.Y;
            //    dbchar.MapPosZ = MapService.Instance.back.Position.Z;
            //}
            //DBService.Instance.Save();

            
            CharacterManager.Instance.RemoveCharacter(character.Id);
            character.Clear();
            MapManager.Instance[character.Info.mapId].CharacterLeave(character);
            
        }
        #endregion
    }
}
