using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIIconItemMe : MonoBehaviour {

	public Image mainImage;
	public Image secondImage;

	public Text mainText;

	public void SetIconItem(string iconName, string text)
	{
		mainImage.overrideSprite = Resloader.Load<Sprite>(iconName);
		mainText.text = text;
	}
}
