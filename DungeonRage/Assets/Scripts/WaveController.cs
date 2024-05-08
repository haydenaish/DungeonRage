using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    [System.Serializable]
    public class EnemyType
    {
        public GameObject prefab;
        public int maxEnemies;

        private int _enemyCount;

        public int EnemyCount
        {
            get { return _enemyCount; }
        }

        public void IncrementEnemyCount()
        {
            _enemyCount++;
        }
    }

    [System.Serializable]
    public class Wave
    {
        public List<EnemyType> enemyTypes;
        public float timeBeforeNextWave;
    }

    public List<Wave> waves;
    private int currentWaveIndex = 0;
    private BoxCollider2D spawnCollider; // Reference to the BoxCollider2D for spawn radius
    private bool playerEntered = false;

    // Event to notify when the WaveManager is activated
    public static event System.Action WaveManagerActivated = delegate { };
    private ScoreManager scoreManager;

    void Start()
    {
        spawnCollider = GetComponent <BoxCollider2D>();
        scoreManager = FindObjectOfType<ScoreManager>(); // Direct reference to ScoreManager
    }

    public void ResetController()
    {
        StopAllCoroutines(); // Stop any running coroutines
        currentWaveIndex = 0; // Reset the wave index
        playerEntered = false; // Reset the playerEntered flag
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerEntered = true;

            // Notify that the WaveManager is activated
            if (WaveManagerActivated != null)
            {
                WaveManagerActivated.Invoke();
            }

            StartCoroutine(SpawnWavesCoroutine());
        }
    }

    private IEnumerator SpawnWavesCoroutine()
    {
        while (currentWaveIndex < waves.Count)
        {
            Wave currentWave = waves[currentWaveIndex];

            yield return new WaitForSeconds(currentWave.timeBeforeNextWave);

            foreach (var enemyType in currentWave.enemyTypes)
            {
                StartCoroutine(SpawnEnemiesCoroutine(enemyType));
            }

            currentWaveIndex++;
        }
    }

    private IEnumerator SpawnEnemiesCoroutine(EnemyType enemyType)
    {
        Bounds bounds = spawnCollider.bounds;

        while (enemyType.EnemyCount < enemyType.maxEnemies)
        {
            // Spawn an enemy
            Vector2 spawnPosition = GetRandomSpawnPosition(bounds);
            Instantiate(enemyType.prefab, spawnPosition, Quaternion.identity);
            enemyType.IncrementEnemyCount();

            // Random interval before spawning the next enemy
            float spawnInterval = Random.Range(.2f, 1f); // Example spawn interval
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private Vector2 GetRandomSpawnPosition(Bounds bounds)
    {
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomY = Random.Range(bounds.min.y, bounds.max.y);

        Vector2 spawnPosition = new Vector2(randomX, randomY);
        return spawnPosition;
    }
}
