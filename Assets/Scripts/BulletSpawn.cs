using UnityEngine;

public class BulletSpawn : MonoBehaviour
{
    public GameObject bulletPrefab;     // BulletPrefab 연결

    public float spawnInterval;         // Bullet Spawn Interval

    private void Start()
    {
        InvokeRepeating(nameof(SpawnBullet), 0f, spawnInterval);
    }

    void SpawnBullet()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies.Length == 0)
        {
            return;
        }

        Instantiate(bulletPrefab, transform.position, Quaternion.identity);
    }

}
