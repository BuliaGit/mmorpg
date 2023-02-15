using Common.Data;
using Managers;
using Models;
using System.Collections;
using UnityEngine;

public class NpcController : MonoBehaviour
{
    public int npcID;

    private SkinnedMeshRenderer render;
    private Color orignColor;
    private Animator anim;

    private NpcDefine npc;
    private bool isInteractive;

    void Start()
    {
        render = this.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        orignColor = render.sharedMaterial.color;

        anim = this.gameObject.GetComponent<Animator>();
        npc = NpcManager.Instance.GetNpcDefine(npcID);

        this.StartCoroutine(Actions()); //闲时待机

        RefreshNpcStatus();
        QuestManager.Instance.onQuestStatusChanged += onQuestStatusChanged;
    }

    private void OnDestroy()
    {
        QuestManager.Instance.onQuestStatusChanged -= onQuestStatusChanged;

        if (UIWorldElementManager.Instance != null)
            UIWorldElementManager.Instance.RemoveNpcQuestStatus(this.transform);
    }

    #region 闲时待机
    IEnumerator Actions()
    {
        while (true)
        {
            if (isInteractive)
                yield return new WaitForSeconds(2f);
            else
                yield return new WaitForSeconds(UnityEngine.Random.Range(5f, 10f));

            anim.SetTrigger("Relax");
        }
    }
    #endregion


    #region NPC任务状态
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
        UIWorldElementManager.Instance.AddNpcQuestStatus(this.transform, questStatus);
    }
    #endregion


    #region 交互
    private void OnMouseDown()
    {
        //if(Vector3.Distance(this.transform.position,User.Instance.CurrentCharacterObject.transform.position)>2f)
        //{
        //    //User.Instance.CurrentCharacterObject.StartNav(this.transform.position);
        //}
        Interactive();
    }

    void Interactive()
    {
        if (!isInteractive)
        {
            isInteractive = true;
            this.StartCoroutine(DoInteractive());
        }
    }

    IEnumerator DoInteractive()
    {
        yield return FaceTolayer();
        if (NpcManager.Instance.Interactive(npc))
        {
            anim.SetTrigger("Talk");
        }
        yield return new WaitForSeconds(3f);    //3秒内点击无效
        isInteractive = false;
    }

    /// <summary>
    /// 转向玩家
    /// </summary>
    /// <returns></returns>
    IEnumerator FaceTolayer()
    {
        Vector3 faceTo = (User.Instance.CurrentCharacterObject.transform.position - this.transform.position).normalized;
        while (Mathf.Abs(Vector3.Angle(this.gameObject.transform.forward, faceTo)) > 5)
        {
            //慢慢转
            this.gameObject.transform.forward = Vector3.Lerp(this.gameObject.transform.forward, faceTo, Time.deltaTime * 5f);
            yield return null;
        }
    }
    #endregion


    #region 高亮
    /// <summary>
    /// 鼠标在上面
    /// </summary>
    private void OnMouseOver()
    {
        Highlight(true);
    }

    private void OnMouseEnter()
    {
        Highlight(true);
    }

    private void OnMouseExit()
    {
        Highlight(false);
    }

    void Highlight(bool highlight)
    {
        if (highlight)
        {
            if (render.sharedMaterial.color != Color.white)
                render.sharedMaterial.color = Color.white;
        }
        else
        {
            if (render.sharedMaterial.color != orignColor)
                render.sharedMaterial.color = orignColor;
        }
    }
    #endregion
}
