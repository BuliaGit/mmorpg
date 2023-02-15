using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINameBarMe : MonoBehaviour {

	public Text roleName;
	public Creature character;
	public UIBuffIcons UIBuff;
	void Start () {
		if (character != null)
		{
			UpdateInfo();
			UIBuff.SetOwner(character);
		}
	}
	
	public void UpdateInfo()
	{
		if(character != null)
		{
			string name = character.Info.Name + " Lv." + character.Info.Level;
			if (name != roleName.text)
			{
                roleName.text = name;
			}
		}
	}
}
