using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Login : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//连接到服务端（这里为本地地址）
		Network.NetClient.Instance.Init("127.0.0.1", 8000);
		Network.NetClient.Instance.Connect();

		//发送信息到服务端
		//SkillBridge.Message.NetMessage msg = new SkillBridge.Message.NetMessage();
		//msg.Request = new SkillBridge.Message.NetMessageRequest();
		//msg.Request.firstRequest = new SkillBridge.Message.FirstTestRequest();
		//msg.Request.firstRequest.Helloworld = "Hello World";

		//Network.NetClient.Instance.SendMessage(msg);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
