using Assets.Scripts.Battle;
using Entities;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
	public class UIWorldEleManagerMe : MonoSingleton<UIWorldEleManagerMe>
	{

		public GameObject nameBarPrefab;
        public GameObject npcStatusPrefab;
        public GameObject popupTextPrefab;

        private Dictionary<Transform, GameObject> uiWorldEles = new Dictionary<Transform, GameObject>();
        private Dictionary<Transform, GameObject> elementStatus = new Dictionary<Transform, GameObject>();


        protected override void OnStart()
        {
            nameBarPrefab.SetActive(false);
            popupTextPrefab.SetActive(false);
        }
        public void AddCharacterNameBar(Transform owner, Creature character)
		{
			GameObject nameBarGo = Instantiate(nameBarPrefab, transform);
			nameBarGo.name = "NameBar" + character.entityId;
			nameBarGo.GetComponent<UIWorldEleMe>().owner = owner;
			nameBarGo.GetComponent<UINameBarMe>().character = character;
			nameBarGo.SetActive(true);
			uiWorldEles[owner] = nameBarGo;
		}

		public void RemoveCharacterNameBar(Transform owner)
		{
			if (uiWorldEles.ContainsKey(owner))
			{
				Destroy(uiWorldEles[owner]);
				uiWorldEles.Remove(owner);
			}
		}

        #region NPC任务状态
        public void AddNpcQuestStatus(Transform owner, NpcQuestStatus status)
        {
            if (this.elementStatus.ContainsKey(owner))
            {
                elementStatus[owner].GetComponent<UIQuestStatus>().SetQuestStatus(status);
            }
            else
            {
                GameObject go = Instantiate(npcStatusPrefab, this.transform);
                go.name = "NpcQuestStatus_" + owner.name;
                go.GetComponent<UIWorldElement>().owner = owner;
                go.GetComponent<UIQuestStatus>().SetQuestStatus(status);
                go.SetActive(true);
                this.elementStatus[owner] = go;
            }
        }

        public void RemoveNpcQuestStatus(Transform owner)
        {
            if (this.elementStatus.ContainsKey(owner))
            {
                Destroy(this.elementStatus[owner]);
                this.elementStatus.Remove(owner);
            }
        }
        #endregion

        public void ShowPopupText(PopupType type,Vector3 position,float damage,bool isCrit)
        {
            GameObject goPopup = Instantiate(popupTextPrefab, position, Quaternion.identity, transform);
            goPopup.name = "Popup";
            goPopup.GetComponent<UIPopupText>().InitPopup(type, damage, isCrit);
            goPopup.SetActive(true);
        }
    }
}