using UnityEngine;
using System.Collections.Generic;

public class EffectManager : MonoBehaviour
{
    public static EffectManager instance;

    [System.Serializable]
    public class EffectPrefab
    {
        public string id; // 예: "PlayerHit", "EnemyAttack", "Explosion"
        public GameObject prefab;
    }

    [Header("이펙트 프리팹 목록")]
    public EffectPrefab[] effectList;
    private Dictionary<string, GameObject> effectMap = new Dictionary<string, GameObject>();

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        // 프리팹 맵 초기화
        foreach (var effect in effectList)
        {
            if (!effectMap.ContainsKey(effect.id))
            {
                effectMap.Add(effect.id, effect.prefab);
            }
        }
    }

    /// <summary>
    /// 지정된 위치에 이펙트를 생성하고 재생합니다. (선택적 부모 지정 가능)
    /// </summary>
    public void PlayEffect(string effectId, Vector3 position, float duration = 1f, Transform parentTransform = null)
    {
        if (effectMap.TryGetValue(effectId, out GameObject prefab))
        {
            GameObject effectObject = Instantiate(prefab, position, Quaternion.identity, parentTransform);

            effectObject.transform.localPosition = effectObject.transform.localPosition;
            effectObject.transform.localRotation = Quaternion.identity;
        }
        else
        {
            Debug.LogWarning($"Effect ID '{effectId}'를 찾을 수 없습니다.");
        }
    }
}