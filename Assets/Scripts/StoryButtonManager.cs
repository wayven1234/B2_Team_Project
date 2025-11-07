using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryButtonManager : MonoBehaviour
{
    public void StorySkipButtonClick()
    {
        SceneManager.LoadScene("Stage1");
    }
}
