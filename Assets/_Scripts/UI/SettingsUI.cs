using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace _Scripts.UI
{
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
        [SerializeField] private Slider _mouseSens;
        [Header("EndingsUI")] 
        [SerializeField] private GameObject _goodVoidImg;
        [SerializeField] private GameObject _badVoidImg;
        [SerializeField] private GameObject _eternityImg;
        [SerializeField] private GameObject _robotsImg;
        
        public static int Graphics
        {
            get { return PlayerPrefs.GetInt("Graphics"); }
            set { PlayerPrefs.SetInt("Graphics", value); QualitySettings.SetQualityLevel(value);}
        }
        public static bool VFXOn
        {
            get { return PlayerPrefs.GetInt("VFXOn") == 1; }
            set { PlayerPrefs.SetInt("VFXOn", value? 1 : 0); }
        }

        private static float Sound
        {
            get { return PlayerPrefs.GetFloat("Sound"); }
            set { PlayerPrefs.SetFloat("Sound", value); }
        }

        private static float Music
        {
            get { return PlayerPrefs.GetFloat("Music"); }
            set { PlayerPrefs.SetFloat("Music", value); }
        }

        private static float Radio
        {
            get { return PlayerPrefs.GetFloat("Radio"); }
            set { PlayerPrefs.SetFloat("Radio", value); }
        }

        public static float MouseSens
        {
            get { return Mathf.Clamp(PlayerPrefs.GetFloat("MouseSens"), 0.05f,0.5f); }
            set { PlayerPrefs.SetFloat("MouseSens", value); }
        }

        public static bool GoodVoid
        {
            get { return PlayerPrefs.GetInt("GoodVoid") == 1; }
            set { PlayerPrefs.SetInt("GoodVoid", 1); }
        }
        public static bool BadVoid
        {
            get { return PlayerPrefs.GetInt("BadVoid") == 1; }
            set { PlayerPrefs.SetInt("BadVoid", 1); }
        }
        public static bool Eternity
        {
            get { return PlayerPrefs.GetInt("Eternity") == 1; }
            set { PlayerPrefs.SetInt("Eternity", 1); }
        }
        public static bool Robots
        {
            get { return PlayerPrefs.GetInt("Robots") == 1; }
            set { PlayerPrefs.SetInt("Robots", 1); }
        }
        public static int CurrentDay
        {
            get { return PlayerPrefs.GetInt("CurrentDay"); }
            set { PlayerPrefs.SetInt("CurrentDay", value); }
        }
        public static int RobotsCount
        {
            get { return PlayerPrefs.GetInt("RobotsCount"); }
            set { PlayerPrefs.SetInt("RobotsCount", value); }
        }
        
        public void Start()
        {
            _graphics.UpdateUI(Graphics);
            _vfx.UpdateUI(VFXOn? 1 : 0);
            _sound.value = Sound;
            _music.value = Music;
            _radio.value = Radio;
            _mouseSens.value = MouseSens;
            
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
    }
}
