using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryButtonManager : MonoBehaviour
{
    public void StorySkipButtonClick()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(SFXType.ButtonClick);
        }
        SceneManager.LoadScene("Stage1");
    }
}
