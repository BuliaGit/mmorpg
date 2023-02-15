using Models;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManagerMe : Singleton<TeamManagerMe> 
{

	public void UpdateTeamInfo(NTeamInfo teamInfo)
	{
		User.Instance.TeamInfo = teamInfo;
		ShowTeamUI(teamInfo != null) ;
	}

	private void ShowTeamUI(bool val)
	{
		if(UIMainMe.Instance != null)
		{
			UIMainMe.Instance.ShowUITeam(val);
		}
	}
}
