using _Scripts.UI;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Zenject;


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

    private static Player _player;

    [Inject]
    private void Initialize(Player player) => _player = player;
    
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
    
    public static int CurrentDay
    {
        get { return PlayerPrefs.GetInt("CurrentDay"); }
        set { PlayerPrefs.SetInt("CurrentDay", value); PlayerPrefs.Save(); }
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
            APIManager.Instance.ChangeCoins(AuthBootstrap.Instance.PlayerName,0);
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
        _player.Cam.cullingMask = value
            ? LayerMask.GetMask("Default", "UI", "Clickable", "Document", "Doors", "VFX")
            : LayerMask.GetMask("Default", "UI", "Clickable", "Document", "Doors");
    }
}

