using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{

    [SerializeField]
    private AudioSource _musicSource, _effectSource;

    [SerializeField]
    private AudioClip _clickSound;

    public static SoundManager instance;

    private bool isMusicMuted;
    private bool IsMusicMuted
    {
        get
        {
            isMusicMuted = PlayerPrefs.HasKey(Constants.Settings.SETTINGS_MUSIC) 
                ? (PlayerPrefs.GetInt(Constants.Settings.SETTINGS_MUSIC) == 0) : false;
            return isMusicMuted;
        }
        set
        {
            PlayerPrefs.SetInt(Constants.Settings.SETTINGS_MUSIC, value ? 0 : 1);
            IsMusicMuted = value;
        }
    }

    private bool isEffectMuted;
    private bool IsEffectMuted
    {
        get
        {
            isEffectMuted = PlayerPrefs.HasKey(Constants.Settings.SETTINGS_SOUND)
                ? (PlayerPrefs.GetInt(Constants.Settings.SETTINGS_SOUND) == 0) : false;
            return isEffectMuted;
        }
        set
        {
            PlayerPrefs.SetInt(Constants.Settings.SETTINGS_SOUND, value ? 0 : 1);
            isEffectMuted = value;
        }
    }

    private bool isShakeMuted;
    private bool IsShakeMuted
    {
        get
        {
            isShakeMuted = PlayerPrefs.HasKey(Constants.Settings.SETTINGS_SHAKE)
                ? (PlayerPrefs.GetInt(Constants.Settings.SETTINGS_SHAKE) == 0) : false;
            return isShakeMuted;
        }
        set
        {
            PlayerPrefs.SetInt(Constants.Settings.SETTINGS_SHAKE, value ? 0 : 1);
            isShakeMuted = value;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        PlayerPrefs.SetInt(Constants.Settings.SETTINGS_MUSIC, IsMusicMuted ? 0 : 1);
        PlayerPrefs.SetInt(Constants.Settings.SETTINGS_SOUND, IsEffectMuted? 0 : 1);
        PlayerPrefs.SetInt(Constants.Settings.SETTINGS_SHAKE, IsShakeMuted? 0 : 1);
        _effectSource.mute = IsEffectMuted;
        _musicSource.mute = IsMusicMuted;

        AddButtonSound();
    }

    public void AddButtonSound()
    {
        var button = FindObjectsOfType<Button>(true);
        foreach (var item in button)
        {
            item.onClick.AddListener(() => {
                PlaySound(_clickSound);
            });
        }
    }

    public void PlaySound(AudioClip clip)
    {
        if (IsEffectMuted) return;
        _effectSource.PlayOneShot(clip);
    }

    public void SetMusic()
    {
        _musicSource.mute = IsMusicMuted;
    }

    public void SetEffect()
    {
        _effectSource.mute = IsEffectMuted;
    }

    public void Vibrate()
    {
        if (IsShakeMuted) return;
        Vibrator.Vibrate();
    }
}
