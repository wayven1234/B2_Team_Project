using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CharacterData
{
    public enum CharacterType { Girl, Boy }

    [Header("캐릭터 기본 정보")]
    public CharacterType characterType;

    [Header("캐릭터 속성")]
    public float maxHealth;
    public float moveSpeed;

    public Dictionary<ItemData.ItemType, int> dictSkillLevel = new Dictionary<ItemData.ItemType, int>();



    /// <summary>
    /// 캐릭터 타입에 따른 스킬 데미지 보너스를 반환합니다.
    /// </summary>
    /// <returns>데미지 보너스 값 (Boy: 3, Girl: 0)</returns>
    public float GetSkillDamageBonus()
    {
        if (characterType == CharacterType.Boy)
            return 3f;
        else
            return 0f;
    }
}