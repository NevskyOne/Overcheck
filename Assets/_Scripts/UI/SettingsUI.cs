using _Scripts.UI;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


public class SettingsUI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private AudioMixer _mixer;
    [Header("SettingsUI")]
    [SerializeField] private Switcher _graphics;
    [SerializeField] private Switcher _vfx;
    [SerializeField] private Slider _sound;
    [SerializeField] private Slider _music;
    [SerializeField] private Slider _radio;
    [SerializeField] private Slider _sfx;
    [SerializeField] private Slider _mouseSens;
    [Header("EndingsUI")] 
    [SerializeField] private GameObject _goodVoidImg;
    [SerializeField] private GameObject _badVoidImg;
    [SerializeField] private GameObject _eternityImg;
    [SerializeField] private GameObject _robotsImg;
    
    
    public static int Graphics
    {
        get { return PlayerPrefs.GetInt("Graphics"); }
        set { PlayerPrefs.SetInt("Graphics", value); PlayerPrefs.Save(); QualitySettings.SetQualityLevel(value);}
    }
    public static bool VFXOn
    {
        get { return PlayerPrefs.GetInt("VFXOn") == 1; }
        set { PlayerPrefs.SetInt("VFXOn", value? 1 : 0); PlayerPrefs.Save();}
    }

    private static float Sound
    {
        get { return PlayerPrefs.GetFloat("Sound"); }
        set { PlayerPrefs.SetFloat("Sound", value); PlayerPrefs.Save();}
    }

    private static float Music
    {
        get { return PlayerPrefs.GetFloat("Music"); }
        set { PlayerPrefs.SetFloat("Music", value); PlayerPrefs.Save();}
    }

    private static float Radio
    {
        get { return PlayerPrefs.GetFloat("Radio"); }
        set { PlayerPrefs.SetFloat("Radio", value); PlayerPrefs.Save();}
    }
    private static float SFX
    {
        get { return PlayerPrefs.GetFloat("SFX"); }
        set { PlayerPrefs.SetFloat("SFX", value); PlayerPrefs.Save();}
    }

    public static float MouseSens
    {
        get { return Mathf.Clamp(PlayerPrefs.GetFloat("MouseSens"), 0.05f,0.5f); }
        set { PlayerPrefs.SetFloat("MouseSens", value); PlayerPrefs.Save();}
    }

    public static bool GoodVoid
    {
        get { return PlayerPrefs.GetInt("GoodVoid") == 1; }
        set { PlayerPrefs.SetInt("GoodVoid", 1); PlayerPrefs.Save();}
    }
    public static bool BadVoid
    {
        get { return PlayerPrefs.GetInt("BadVoid") == 1; }
        set { PlayerPrefs.SetInt("BadVoid", 1); PlayerPrefs.Save();}
    }
    public static bool Eternity
    {
        get { return PlayerPrefs.GetInt("Eternity") == 1; }
        set { PlayerPrefs.SetInt("Eternity", 1); PlayerPrefs.Save();}
    }
    public static bool Robots
    {
        get { return PlayerPrefs.GetInt("Robots") == 1; }
        set { PlayerPrefs.SetInt("Robots", 1); PlayerPrefs.Save();}
    }
    public static int CurrentDay
    {
        get { return PlayerPrefs.GetInt("CurrentDay"); }
        set { PlayerPrefs.SetInt("CurrentDay", value); PlayerPrefs.Save(); }
    }
    public static int RobotsCount
    {
        get { return PlayerPrefs.GetInt("RobotsCount"); }
        set { PlayerPrefs.SetInt("RobotsCount", value); PlayerPrefs.Save();}
    }
    
    public void Start()
    {
        _graphics.UpdateUI(Graphics);
        _vfx.UpdateUI(VFXOn? 1 : 0);
        if (PlayerPrefs.GetInt("InGame") == 0)
        {
            ChangeVolume(0.5f);
            ChangeMusic(0.5f);
            ChangeRadio(0.5f);
            ChangeSFX(0.5f);
            APIManager.Instance.ChangeCoins(Bootstrap.Instance.PlayerName,0);
            PlayerPrefs.SetInt("InGame", 1);
            PlayerPrefs.Save();
        }
        else
        {
            _sound.value = Sound;
            _music.value = Music;
            _radio.value = Radio;
            _sfx.value = Radio;
            _mouseSens.value = MouseSens;
        }

        if(GoodVoid) _goodVoidImg.SetActive(true);
        if(BadVoid) _badVoidImg.SetActive(true);
        if(Eternity) _eternityImg.SetActive(true);
        if(Robots) _robotsImg.SetActive(true);
    }

    public void ChangeVolume(float value)
    {
        _mixer.SetFloat("Master", Mathf.Log10(value) * 20f);
        Sound = value;
    }

    public void ChangeMusic(float value)
    {
        _mixer.SetFloat("Music", Mathf.Log10(value) * 20f);
        Music = value;
    }

    public void ChangeRadio(float value)
    {
        _mixer.SetFloat("Radio", Mathf.Log10(value) * 20f);
        Radio = value;
    }
    
    public void ChangeSFX(float value)
    {
        _mixer.SetFloat("SFX", Mathf.Log10(value) * 20f);
        SFX = value;
    }

    public static void ChangeVFX(bool value)
    {
        VFXOn = value;
        PlayerInteractions.DefaultMask = value
            ? LayerMask.GetMask("Default", "UI", "Clickable", "Document", "NPCObject", "Doors", "VFX", "DocPlace")
            : LayerMask.GetMask("Default", "UI", "Clickable", "Document", "NPCObject", "Doors", "DocPlace");
    }
}

