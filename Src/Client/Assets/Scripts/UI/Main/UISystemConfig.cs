using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISystemConfig : UIWindow
{
    [SerializeField] Image musicOff;
    [SerializeField] Image sfxOff;

    [SerializeField] Toggle toggleMusic;
    [SerializeField] Toggle toggleSfx;

    [SerializeField] Slider silderMusic;
    [SerializeField] Slider silderSfx;

    private void Start()
    {
        this.toggleMusic.onValueChanged.AddListener(MusicToggle);
        this.toggleSfx.onValueChanged.AddListener(SfxToggle);
        this.silderMusic.onValueChanged.AddListener(MusicVolume);
        this.silderSfx.onValueChanged.AddListener(SfxVolume);

        this.toggleMusic.isOn = Config.MusicOn;
        this.toggleSfx.isOn = Config.SfxOn;
        this.silderMusic.value = Config.MusicVolume;
        this.silderSfx.value = Config.SfxVolume;
    }

    public override void OnYesClike()
    {
        SoundManager.Instance.PlaySfx(SoundDefine.SFX_UI_Click);
        PlayerPrefs.Save();
        base.OnYesClike();
    }

    void MusicToggle(bool on)
    {
        musicOff.enabled = !on;
        Config.MusicOn = on;
        SoundManager.Instance.PlaySfx(SoundDefine.SFX_UI_Click);
    }
    void SfxToggle(bool on)
    {
        sfxOff.enabled = !on;
        Config.SfxOn = on;
        SoundManager.Instance.PlaySfx(SoundDefine.SFX_UI_Click);
    }

    void MusicVolume(float volume)
    {
        Config.MusicVolume = (int)volume;
        PlaySound();
    }
    void SfxVolume(float volume)
    {
        Config.SfxVolume = (int)volume;
        PlaySound();
    }

    float lastPlayTime = 0;
    private void PlaySound()
    {
        if(Time.realtimeSinceStartup - lastPlayTime > 0.1f)
        {
            lastPlayTime = Time.realtimeSinceStartup;
            SoundManager.Instance.PlaySfx(SoundDefine.SFX_UI_Click);
        }
    }
}
