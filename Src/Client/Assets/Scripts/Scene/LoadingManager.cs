using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

using SkillBridge.Message;
using ProtoBuf;
using Services;
using Managers;
using Assets.Scripts.Services;

public class LoadingManager : MonoBehaviour {

    public GameObject UITips;
    public GameObject UILoading;
    public GameObject UILogin;

    public Slider progressBar;
    public Text progressText;
    public Text progressNumber;

    // Use this for initialization
    IEnumerator Start()
    {
        log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("log4net.xml"));
        UnityLogger.Init();
        Common.Log.Init("Unity");
        Common.Log.Info("LoadingManager start");

        UITips.SetActive(true);
        UILoading.SetActive(false);
        UILogin.SetActive(false);
        yield return new WaitForSeconds(2f);
        UILoading.SetActive(true);
        yield return new WaitForSeconds(1f);
        UITips.SetActive(false);

        yield return DataManager.Instance.LoadData();

        //Init basic services
        //地图
        MapService.Instance.Init();
        //用户
        UserService.Instance.Init();
        //道具
        ItemService.Instance.Init();
        //商店
        ShopManager.Instance.Init();
        //状态
        StatusService.Instance.Init();
        //好友
        FriendService.Instance.Init();
        //组队
        TeamService.Instance.Init();
        //公会
        GuildService.Instance.Init();
        //聊天
        ChatService.Instance.Init();
        //战斗
        BattleService.Instance.Init();

        SoundManager.Instance.PlayMusic(SoundDefine.Music_Login);

        // Fake Loading Simulate
        //for (float i = 50; i < 100;)
        //{
        //    i += Random.Range(0.1f, 1.5f);
        //    progressBar.value = i;
        //    progressNumber.text = string.Format("{0}%", progressBar.value);
        //    yield return new WaitForEndOfFrame();
        //}

        for (float i = 0; i < 100;i++)
        {
            progressBar.value = i;
            progressNumber.text = string.Format("{0}%", progressBar.value);
            yield return new WaitForEndOfFrame();
        }

        UILoading.SetActive(false);
        UILogin.SetActive(true);
        yield return null;
    }
}
