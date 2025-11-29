using UnityEngine;

public class VFXDestructor : MonoBehaviour
{
    public void DestroyOnAnimationEnd()
    {
        Destroy(gameObject);
    }
}
