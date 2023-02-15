using Common.Data;
using Managers;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class NpcControllerMe : MonoBehaviour {

	public int npcID;
	private Animator anim;
	private NpcDefine npcDefine;
	private bool isInteractive = false;
	private SkinnedMeshRenderer skinRender;
	private Color originColor;

    public void Start()
	{
		anim = GetComponent<Animator>();
		npcDefine = NpcManagerMe.Instance.GetNpcDefine(npcID);
        skinRender = GetComponentInChildren<SkinnedMeshRenderer>();
        originColor = skinRender.sharedMaterial.color;
		NpcManagerMe.Instance.UpdateNpcPosition(npcID, transform.position);
		StartCoroutine(Actions());  //闲时待机

        RefreshNpcStatus();
        QuestManager.Instance.onQuestStatusChanged += onQuestStatusChanged;
    }

    private void OnDestroy()
    {
        QuestManager.Instance.onQuestStatusChanged -= onQuestStatusChanged;

        if (UIWorldEleManagerMe.Instance != null)
            UIWorldEleManagerMe.Instance.RemoveNpcQuestStatus(this.transform);
    }

    #region NPC状态
    void onQuestStatusChanged(Quest quest)
    {
        this.RefreshNpcStatus();
    }

    /// <summary>
    /// 刷新NPC任务状态
    /// </summary>
    private void RefreshNpcStatus()
    {
        NpcQuestStatus questStatus = QuestManager.Instance.GetQuestStatusByNpc(this.npcID);
        UIWorldEleManagerMe.Instance.AddNpcQuestStatus(this.transform, questStatus);
    }

    #endregion

    #region 闲时待机
    private IEnumerator Actions()
	{
		while (true)
		{
			if (isInteractive)
				yield return new WaitForSeconds(2);
			else
				yield return new WaitForSeconds(UnityEngine.Random.Range(10, 15));

            anim.SetTrigger("Relax");
        }
	}
	#endregion

	#region 交互
	public void Interactive()
	{
		if (!isInteractive)
		{
			isInteractive = true;
			StartCoroutine(DoInteractive());
		}
	}

	
	private IEnumerator DoInteractive()
	{
		yield return StartCoroutine(FaceToPlayer());
		if (NpcManagerMe.Instance.Interactive(npcDefine))
		{
			anim.SetTrigger("Talk");
		}
		yield return new WaitForSeconds(3);
		isInteractive = false;
	}

	private IEnumerator FaceToPlayer()
	{
		Vector3 faceTo = (User.Instance.CurrentCharacterObject.transform.position - transform.position).normalized;
		while (Mathf.Abs(Vector3.Angle(gameObject.transform.forward, faceTo)) > 5)
		{
			gameObject.transform.forward = Vector3.Lerp(gameObject.transform.forward, faceTo, Time.deltaTime * 5);
			yield return null;
		}
	}

	/// <summary>
	/// 点击NPC交互
	/// </summary>
	public void OnMouseDown()
	{
		Interactive();

    }
    #endregion

    #region 高亮显示

    public void OnMouseOver()
	{
		HighLight(true);
	}
	public void OnMouseEnter()
	{
		HighLight(true);
	}
	private void OnMouseExit()
	{
		HighLight(false);
	}

	private void HighLight(bool highlight)
	{
		if (highlight)
		{
			if (skinRender.sharedMaterial.color != Color.white)
			{
				skinRender.sharedMaterial.color = Color.white;
			}
		}
        else
        {
            if (skinRender.sharedMaterial.color != originColor)
            {
                skinRender.sharedMaterial.color = originColor;
            }
        }
    }
	#endregion
}
