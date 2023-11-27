using UnityEngine;

public class Config
{
    public static bool MusicOn
    {
        get { return PlayerPrefs.GetInt("Music", 1) == 1; }
        set
        {
            PlayerPrefs.SetInt("Music", value ? 1 : 0);
            SoundManager.Instance.MusicOn = value;
        }
    }
    public static bool SfxOn
    {
        get { return PlayerPrefs.GetInt("SFX", 1) == 1; }
        set
        {
            PlayerPrefs.SetInt("SFX", value ? 1 : 0);
            SoundManager.Instance.SfxOn = value;
        }
    }

    public static int MusicVolume
    {
        get { return PlayerPrefs.GetInt("MusicVolum", 100); }
        set
        {
            PlayerPrefs.SetInt("MusicVolum", value);
            SoundManager.Instance.MusicVolum = value;
        }
    }
    public static int SfxVolume
    {
        get { return PlayerPrefs.GetInt("SfxVolum", 100); }
        set
        {
            PlayerPrefs.SetInt("SfxVolum", value);
            SoundManager.Instance.SfxVolum = value;
        }
    }

    ~Config()
    {
        PlayerPrefs.Save();
    }
}
