using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class StoryManager : MonoBehaviour
{
    [Header("스토리 컷 (이미지 Object)")]
    public GameObject[] _storyCuts;

    [Header("컷 전환 시간 (초)")]
    public float _cutDelay;

    [Header("페이드 속도")]
    public float _fadeSpeed;

    [Header("페이드용 검성 Image")]
    public Image _fadePanel;

    private int _cutCount = 0;

    private void Start()
    {
        foreach (GameObject cut in _storyCuts)
        {
            cut.SetActive(false);
        }
        _storyCuts[0].SetActive(true);

        _fadePanel.color = new Color(0, 0, 0, 1);

        StartCoroutine(PlayStory());
    }

    IEnumerator PlayStory()
    {
        yield return StartCoroutine(Fade(1, 0));

        while (_cutCount < _storyCuts.Length)
        {
            yield return new WaitForSeconds(_cutDelay);

            if (_cutCount < _storyCuts.Length - 1)
            {
                yield return StartCoroutine(Fade(0, 1));

                _storyCuts[_cutCount].SetActive(false);
                _cutCount++;
                _storyCuts[_cutCount].SetActive(true);

                yield return StartCoroutine(Fade(1, 0));
            }
            else 
            {
                yield return StartCoroutine(Fade(0, 1));
                SceneManager.LoadScene("Stage1");
            }
        }
    }

    IEnumerator Fade(float start, float end)
    {
        float t = 0f;
        Color color = _fadePanel.color;

        while (t < 1f)
        {
            t += Time.deltaTime * _fadeSpeed;
            float alpha = Mathf.Lerp(start, end, t);
            _fadePanel.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        _fadePanel.color = new Color(color.r, color.g, color.b, end);
    }
}
