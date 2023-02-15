using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestItemMe : ListViewMe.ListViewItemMe
{
	public Quest ownerQuest;
	public Text questItemTxt;

	public void SetQuestItemTxt(Quest quest)
	{
        ownerQuest = quest;
		string typeName = quest.Define.Type == Common.Data.QuestType.Main ? "主线" : "支线";

		questItemTxt.text = string.Format("【{0}】{1}", typeName, quest.Define.Name);
	}
	
}
