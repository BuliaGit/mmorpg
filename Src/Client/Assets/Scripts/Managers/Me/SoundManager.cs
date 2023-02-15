using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoSingleton<SoundManager> 
{
	public AudioMixer audioMixer;
	public AudioSource musicAudioSource;
	public AudioSource soundAudioSource;

	const string MusicPath = "Music/";
	const string SoundPath = "Sound/";

	private bool musicOn;
    public bool MusicOn
	{
		get { return musicOn; }
		set
		{
			musicOn = value;
			MusicMute(!musicOn);
		}
	}

	private bool soundOn;
	public bool SoundOn
	{
		get { return soundOn; }
		set
		{
			soundOn = value;
			SoundMute(!soundOn);
		}
	}

	private int musicVolume;
	public int MusicVolume
	{
		get { return musicVolume; }
		set
		{
			if(musicVolume != value)
			{
				musicVolume = value;
				if (musicOn)
				{
					SetVolume("MusicVolume", musicVolume);
				}
			}
		}
	}
	private int soundVolume;
	public int SoundVolume
	{
		get { return soundVolume; }
		set
		{
			if(soundVolume != value)
			{
				soundVolume = value;
				if (soundOn)
				{
					SetVolume("SoundVolume", soundVolume);
				}
			}
		}
	}

	void Start()
	{
		MusicVolume = Config.MusicVolume;
		SoundVolume = Config.SoundVolume;
		MusicOn = Config.MusicOn;
		SoundOn = Config.SoundOn;
	}

    private void MusicMute(bool val)
    {
		SetVolume("MusicVolume", val ? 0 : musicVolume);
    }

    private void SoundMute(bool val)
    {
        SetVolume("SoundVolume", val ? 0 : soundVolume);
    }

	private void SetVolume(string name,int value)
	{
		float volume = value * 0.5f - 50f;
		audioMixer.SetFloat(name, volume);
	}

    public void PlayMusic(string musicName)
    {
		AudioClip clip = Resloader.Load<AudioClip>(MusicPath + musicName);
		if(clip == null)
		{
			Debug.LogWarningFormat("PlayMusic:{0} not existed", musicName);
			return;
		}
		if (musicAudioSource.isPlaying)
		{
			musicAudioSource.Stop();
		}
		musicAudioSource.clip = clip;
		musicAudioSource.Play();
    }

    public void PlaySound(string soundName)
    {
        AudioClip clip = Resloader.Load<AudioClip>(SoundPath + soundName);
        if (clip == null)
        {
            Debug.LogWarningFormat("PlaySound:{0} not existed", soundName);
            return;
        }
		soundAudioSource.PlayOneShot(clip);
    }

}
