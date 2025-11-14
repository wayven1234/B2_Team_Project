using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public GameObject weaponPrefab; // Weapon 스크립트가 붙어있는 프리팹
    public ItemData bookData;       // 인스펙터에서 BookData.asset 연결

    private Weapon currentWeapon;

    void Start()
    {
        // "Book" 무기를 레벨 0으로 생성
        GameObject weaponObj = Instantiate(weaponPrefab, transform);
        currentWeapon = weaponObj.GetComponent<Weapon>();
        currentWeapon.Init(bookData);
    }

    // 레벨 업 버튼 등에서 이 함수를 호출
    public void UpgradeWeapon()
    {
        //if (currentWeapon != null)
        //{
        //    currentWeapon.LevelUp();
        //}
    }
}