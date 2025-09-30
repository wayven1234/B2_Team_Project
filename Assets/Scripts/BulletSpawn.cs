using UnityEngine;

public class BulletSpawn : MonoBehaviour
{
    public GameObject bulletPrefab;

    public float spawnInterval;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnBullet), 0f, spawnInterval);
    }

    void SpawnBullet()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        //Debug.Log("Enemy count: " + enemies.Length);

        if (enemies.Length == 0)
        {
            //Debug.Log("Enemy 없음, Bullet 생성 안 함");
            return;
        }

        //Debug.Log("Enemy 있음, Bullet 생성");
        Instantiate(bulletPrefab, transform.position, Quaternion.identity);
    }

}
