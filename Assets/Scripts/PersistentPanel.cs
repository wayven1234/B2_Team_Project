using UnityEngine;

public class PersistentPanel : MonoBehaviour
{
    private void Awake()
    {
        PersistentPanel[] existingPanels = FindObjectsByType<PersistentPanel>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        if (existingPanels.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
}