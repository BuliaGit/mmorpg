using Assets.Scripts.Entities;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRoleInfo : MonoBehaviour
{

    public Slider hpSlider;
    public Slider mpSlider;

    // Update is called once per frame
    void Update()
    {
        hpSlider.value = User.Instance.CurrentCharacter.Attributes.MP / User.Instance.CurrentCharacter.Attributes.MaxHP;
        mpSlider.value = User.Instance.CurrentCharacter.Attributes.MP / User.Instance.CurrentCharacter.Attributes.MaxMP;

    }
}
