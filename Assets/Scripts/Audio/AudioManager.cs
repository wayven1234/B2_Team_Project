using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using static UnityEngine.Rendering.DebugUI;
using System.Xml.Serialization;


public enum BGMType { Title, Battle, Boss, Victory, GameOver }

public enum SFXType { UIButton, SummonButton, knightAttack, SwordknightAttack, ArcherAttack, CastleAttack, BaseAttack, BossAttack }

[System.Serializable]
public class BGMClipData
{
    public BGMType type;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 0.5f;
}

[System.Serializable]
public class SFXClipData
{
    public SFXType type;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 1f;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    [Header("Mixer Croups")]
    [SerializeField] AudioMixerGroup BGMMixerGroup;
    [SerializeField] AudioMixerGroup SFXMixerGroup;

    [Header("BGM Clips")]
    public BGMClipData[] BGMClips;

    [Header("SFX Clips")]
    public SFXClipData[] SFXClips;

    private Dictionary<BGMType, BGMClipData> bgmDict;
    private Dictionary<SFXType, SFXClipData> sfxDict;

    private AudioSource BGMPlayer;
    private AudioSource SFXPlayer;

    private BGMType currentBGMType;
    private bool isFirstPlay = true;

    private string lastSceneName = "";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Init();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Init()
    {
        if (BGMPlayer != null && SFXPlayer != null) return;


        Debug.Log("[AudioManager] Init() 실행");

        // AudioSource 생성
        GameObject bgmObj = new GameObject("BGMPlayer");
        bgmObj.transform.SetParent(transform);
        BGMPlayer = bgmObj.AddComponent<AudioSource>();
        BGMPlayer.outputAudioMixerGroup = BGMMixerGroup;
        BGMPlayer.loop = true;
        BGMPlayer.playOnAwake = false;
        BGMPlayer.ignoreListenerPause = true;

        GameObject sfxObj = new GameObject("SFXPlayer");
        sfxObj.transform.SetParent(transform);
        SFXPlayer = sfxObj.AddComponent<AudioSource>();
        SFXPlayer.outputAudioMixerGroup = SFXMixerGroup;
        SFXPlayer.loop = false;
        SFXPlayer.playOnAwake = false;

        if (bgmDict == null)
        {
            bgmDict = new Dictionary<BGMType, BGMClipData>();
            foreach (var data in BGMClips)
            {
                if (!bgmDict.ContainsKey(data.type) && data.clip != null)
                {
                    bgmDict.Add(data.type, data);
                }
            }
        }

        if (sfxDict == null)
        {
            sfxDict = new Dictionary<SFXType, SFXClipData>();
            foreach (var data in SFXClips)
            {
                if (!sfxDict.ContainsKey(data.type) && data.clip != null)
                {
                    sfxDict.Add(data.type, data);
                }
            }
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Init();

        lastSceneName = scene.name;

        switch (scene.name)
        {
            case "StartScene":
                ;
                PlayBGM(BGMType.Title, forceRestart: true);
                break;

            case "InGameScene":
                ;
                PlayBGM(BGMType.Battle, forceRestart: true);
                break;
        }
    }


    // ========== BGM ==========
    public void PlayBGM(BGMType type, bool forceRestart = false, bool useFade = true)
    {
        Debug.Log(type.ToString());
        Debug.Log(Time.timeScale);

        if (!bgmDict.TryGetValue(type, out var newClipData) || newClipData.clip == null)
        {
            Debug.LogWarning($"[AudioManager] 등록되지 않은 BGM 타입 또는 클립이 null: {type}");
            return;
        }

        if (!forceRestart && BGMPlayer.clip == newClipData.clip && BGMPlayer.isPlaying)
        {
            Debug.Log("[PlayBGM] 동일한 BGMType 재생 중, 스킵");
            return;
        }

        //bool isSameClip = BGMPlayer.clip == newClipData.clip;

        if (!isFirstPlay && BGMPlayer.isPlaying && BGMPlayer.clip == newClipData.clip)
        {
            Debug.Log("[동일한 BGM이 이미 재생 중");
            return;
        }

        if (useFade && BGMPlayer.isPlaying) // 페이드 효과를 사용하고, 현재 BGM이 재생 중일 때만 페이드 인 아웃 효과 적용
        {
            StartCoroutine(FadeInOutRoutine(newClipData));
        }
        else // BGM이 재생중이 아니거나, 페이드 효과를 사용하지 않을 경우 즉시 교체
        {
            BGMPlayer.Stop();
            BGMPlayer.clip = newClipData.clip;
            BGMPlayer.volume = 0f;
            BGMPlayer.Play();
        }

        currentBGMType = type;
        StopAllCoroutines();
        StartCoroutine(FadeInOutRoutine(newClipData));
        //isFirstPlay = false;
    }

    IEnumerator FadeInOutRoutine(BGMClipData newClipData)
    {
        yield return StartCoroutine(MusicFadeout());

        BGMPlayer.clip = newClipData.clip;
        BGMPlayer.Play();

        yield return StartCoroutine(MusicFadein(newClipData.volume));
    }

    IEnumerator MusicFadein(float targetVolume)
    {
        float duration = 0.5f;
        float t = 0f;

        while (t < duration)
        {
            BGMPlayer.volume = Mathf.Lerp(0f, targetVolume, t / duration);
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        BGMPlayer.volume = targetVolume;
    }

    IEnumerator MusicFadeout()
    {
        float duration = 0.5f;
        float t = 0f;
        float startVol = BGMPlayer.volume;

        while (t < duration)
        {
            BGMPlayer.volume = Mathf.Lerp(startVol, 0f, t / duration);
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        BGMPlayer.volume = 0f;
        BGMPlayer.Stop();
    }

    private IEnumerator SwitchBGMCoroutine(BGMClipData newClipData, bool useFade)
    {
        if (useFade && BGMPlayer.isPlaying)
        {
            yield return StartCoroutine(MusicFadeout());
        }
    }

    public void StopBGM() => BGMPlayer.Stop();


    // ========== SFX ==========
    public void PlaySFX(SFXType type)
    {
        if (!sfxDict.ContainsKey(type))
        {
            Debug.LogWarning("등록되지 않은 SFX 타입입니다:" + type);
            return;
        }

        var clipData = sfxDict[type];
        SFXPlayer.PlayOneShot(clipData.clip, clipData.volume);
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        SFXPlayer.PlayOneShot(clip, volume);
    }
}
