using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class AudioUIManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    private const float MIN_DB = -80f;

    public void Start()
    {
        Debug.Log($"[AudioUIManager] Start executed in scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");

        float master = PlayerPrefs.GetFloat("Master", 0.50005f);
        float bgm = PlayerPrefs.GetFloat("BGM", 0.50005f);
        float sfx = PlayerPrefs.GetFloat("SFX", 0.50005f);

        Debug.Log($"[AudioUIManager] Loaded Prefs: Master={master}, BGM={bgm}, SFX={sfx}");

        masterSlider.value = master;
        bgmSlider.value = bgm;
        sfxSlider.value = sfx;

        SetMasterVolume(master);
        SetBGMVolume(bgm);
        SetSFXVolume(sfx);

        Debug.Log("[AudioUIManager] Event listeners added.");

        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMasterVolume(float value)
    {
        float db;
        if (value <= 0.0001f)
        {
            db = MIN_DB;
        }
        else
        {
            db = Mathf.Log10(value) * 20;
        }

        audioMixer.SetFloat("Master", db);
        PlayerPrefs.SetFloat("Master", value);
        PlayerPrefs.Save();
    }

    public void SetBGMVolume(float value)
    {
        float db;
        if (value <= 0.0001f)
        {
            db = MIN_DB;
        }
        else
        {
            db = Mathf.Log10(value) * 20;
        }

        audioMixer.SetFloat("BGM", db);
        PlayerPrefs.SetFloat("BGM", value);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        float db;
        if (value <= 0.0001f)
        {
            db = MIN_DB;
        }
        else
        {
            db = Mathf.Log10(value) * 20;
        }

        audioMixer.SetFloat("SFX", db);
        PlayerPrefs.SetFloat("SFX", value);
        PlayerPrefs.Save();
    }
}