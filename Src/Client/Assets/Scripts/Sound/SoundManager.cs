using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoSingleton<SoundManager>
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] AudioSource musicAudioSource;
    [SerializeField] AudioSource sfxAudioSources;

    const string MusicPath = "Music/";
    const string SfxPath = "SFX/";

    private bool musicOn;
    public bool MusicOn
    {
        get { return musicOn; }
        set
        {
            musicOn = value;
            this.MusicMute(!musicOn);
        }
    }
    private bool sfxOn;
    public bool SfxOn
    {
        get { return sfxOn; }
        set
        {
            sfxOn = value;
            this.SfxMute(!sfxOn);
        }
    }

    private int musicVolum;
    public int MusicVolum
    {
        get { return musicVolum; }
        set
        {
            if(musicVolum != value)
            {
                musicVolum = value;
                if (musicOn)
                    MusicMute(false);
            }
        }
    }
    private int sfxVolum;
    public int SfxVolum
    {
        get { return sfxVolum; }
        set
        {
            if(sfxVolum != value)
            {
                sfxVolum = value;
                if (sfxOn)
                    SfxMute(false);
            }
        }
    }

    private void Start()
    {
        this.MusicVolum = Config.MusicVolume;
        this.sfxVolum = Config.SfxVolume;
        this.musicOn = Config.MusicOn;
        this.sfxOn = Config.SfxOn;
    }

    void MusicMute(bool mute)
    {
        SetVolum("MusicVolume", mute ? 0 : musicVolum);
    }
    void SfxMute(bool mute)
    {
        SetVolum("SfxVolume", mute ? 0 : sfxVolum);
    }

    void SetVolum(string name, int value)
    {
        float volum = value * 0.5f - 50f;
        this.audioMixer.SetFloat(name, volum);
    }

    public void PlayMusic(string name)
    {
        AudioClip clip = Resloader.Load<AudioClip>(MusicPath + name);
        if(clip == null)
        {
            Debug.LogWarningFormat("PlayerMusic: {0} not existed", name); return;
        }
        if (musicAudioSource.isPlaying)
            musicAudioSource.Stop();

        musicAudioSource.clip = clip;
        musicAudioSource.Play();
    }
    public void PlaySfx(string name)
    {
        AudioClip clip = Resloader.Load<AudioClip>(SfxPath + name);
        if (clip == null)
        {
            Debug.LogWarningFormat("PlayerSFX: {0} not existed", name); return;
        }
        sfxAudioSources.PlayOneShot(clip);
    }

    protected void PlayClipOnAudioSource(AudioSource source, AudioClip clip, bool isLoop)
    {

    }
}
