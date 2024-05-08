using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [System.Serializable]
    public class EnemyType
    {
        public GameObject prefab;
        public int maxEnemies;

        private int _enemyCount; // private field for enemy count

        public int EnemyCount // public property with a getter
        {
            get { return _enemyCount; }
        }

        public void IncrementEnemyCount()
        {
            _enemyCount++;
        }
    }

    public Collider2D roomCollider;
    public float minSpawnInterval = 1f;
    public float maxSpawnInterval = 3f;

    public bool isBoss = false;

    public List<EnemyType> enemyTypes;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Trigger player test");
            if (isBoss)
            {
                //Debug.Log("Trigger player is boss test");
                BossController.setAttacking();
            }
            else
            {
                foreach (var enemyType in enemyTypes)
                {
                    StartCoroutine(SpawnEnemiesCoroutine(enemyType));
                }
            }
        }
    }

    private IEnumerator SpawnEnemiesCoroutine(EnemyType enemyType)
    {
        Bounds bounds = roomCollider.bounds;

        while (enemyType.EnemyCount < enemyType.maxEnemies)
        {
            // Spawn an enemy
            Vector2 spawnPosition = GetRandomSpawnPosition(bounds);
            Instantiate(enemyType.prefab, spawnPosition, Quaternion.identity);
            enemyType.IncrementEnemyCount(); // Increment the private enemy count

            // Random interval before spawning the next enemy
            float spawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public Vector2 GetRandomSpawnPosition(Bounds bounds)
    {
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomY = Random.Range(bounds.min.y, bounds.max.y);

        Vector2 spawnPosition = new Vector2(randomX, randomY);
        return spawnPosition;
    }
}
