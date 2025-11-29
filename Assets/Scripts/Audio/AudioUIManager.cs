using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class AudioUIManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    public void Start()
    {
        float master = PlayerPrefs.GetFloat("Master", 0.50005f);
        float bgm = PlayerPrefs.GetFloat("BGM", 0.50005f);
        float sfx = PlayerPrefs.GetFloat("SFX", 0.50005f);

        masterSlider.value = master;
        bgmSlider.value = bgm;
        sfxSlider.value = sfx;

        SetMasterVolume(master);
        SetBGMVolume(bgm);
        SetSFXVolume(sfx);

        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMasterVolume(float value)
    {
        audioMixer.SetFloat("Master", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("Master", value);
        PlayerPrefs.Save(); // 명시적 저장
    }

    public void SetBGMVolume(float value)
    {
        audioMixer.SetFloat("BGM", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("BGM", value);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat("SFX", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("SFX", value);
        PlayerPrefs.Save();
    }
}