using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class UIWindowMe : MonoBehaviour 
{
	public GameObject Root;

	public delegate void CloseHandler(UIWindowMe sender,WindowResult result);

	public event CloseHandler OnClose;
	public virtual Type Type { get { return this.GetType(); } }
	public enum WindowResult
	{
		None = 0,
		Yes,
		No
	}

	public void Close(WindowResult result = WindowResult.None)
	{
		UIManagerMe.Instance.Close(Type);
		if (OnClose != null)
		{
			OnClose(this,result);
		}
		OnClose = null;
	}

	public virtual void OnCloseClick()
	{
		Close();
	}

	public virtual void OnYesClick()
	{
		Close(WindowResult.Yes);
	}

	public virtual void OnNoClick()
	{
		Close(WindowResult.No);
	}

}
