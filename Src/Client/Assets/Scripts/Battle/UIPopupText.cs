using Assets.Scripts.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupText : MonoBehaviour
{
    public Text normalDamageText;
    public Text critDamageText;
    public Text healText;
    public float floatTime = 0.5f;

    public void InitPopup(PopupType type, float number, bool isCrit)
    {
        string text = number.ToString("0");
        normalDamageText.text = text;
        critDamageText.text = text;
        healText.text = text;

        normalDamageText.enabled = !isCrit && number < 0;
        critDamageText.enabled = isCrit && number < 0;
        healText.enabled = number > 0;

        float time = Random.Range(0, 0.5f) + floatTime;

        float height = Random.Range(0.5f, 1);
        float disperse = Random.Range(-0.5f, 0.5f);
        disperse += Mathf.Sign(disperse) * 0.3f;

        LeanTween.moveX(gameObject, transform.position.x + disperse, time);
        LeanTween.moveZ(gameObject, transform.position.z + disperse, time);
        LeanTween.moveY(gameObject, transform.position.y + height, time).setEaseOutBack().setDestroyOnComplete(true);
    }
}