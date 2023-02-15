using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISystemConfig : UIWindowMe
{
    public Image musicOff;
    public Image soundOff;

    public Toggle toggleMusic;
    public Toggle toggleSound;

    public Slider sliderMusic;
    public Slider sliderSound;

    public void Start()
    {
        toggleMusic.isOn = Config.MusicOn;
        toggleSound.isOn = Config.SoundOn;
        sliderMusic.value = Config.MusicVolume;
        sliderSound.value = Config.SoundVolume;
    }

    public override void OnYesClick()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
        PlayerPrefs.Save();
        base.OnYesClick();
    }

    public void OnMusicToggle(bool isOn)
    {
        musicOff.enabled = !isOn;
        Config.MusicOn = isOn;
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
    }
    public void OnSoundToggle(bool isOn)
    {
        soundOff.enabled = !isOn;
        Config.SoundOn = isOn;
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
    }

    public void OnMusicSlider(float value)
    {
        Config.MusicVolume = (int)value;
        PlaySound();
    }

    public void OnSoundSlider(float value)
    {
        Config.SoundVolume = (int)value;
        PlaySound();
    }

    float lastPlay = 0;
    private void PlaySound()
    {
        if(Time.realtimeSinceStartup - lastPlay > 0.1)
        {
            lastPlay = Time.realtimeSinceStartup;
            SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
        }
    }
}
