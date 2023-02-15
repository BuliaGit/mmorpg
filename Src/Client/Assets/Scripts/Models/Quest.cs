using Common.Data;
using SkillBridge.Message;

namespace Models
{
    /// <summary>
    /// 任务类
    /// </summary>
    public class Quest
    {
        /// <summary>
        /// 本地配置任务信息
        /// </summary>
        public QuestDefine Define;

        /// <summary>
        /// 网络任务信息
        /// </summary>
        public NQuestInfo Info;


        public Quest()
        {

        }
        public Quest(NQuestInfo info)
        {
            this.Info = info;
            this.Define = DataManager.Instance.Quests[info.QuestId];
        }

        public Quest(QuestDefine define)
        {
            this.Define = define;
            this.Info = null;
        }

        public string GetTypeName()
        {
            return EnumUtil.GetEnumDescription(this.Define.Type);
        }
    }
}