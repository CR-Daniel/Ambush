using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    private Coroutine spawnCoroutine;

    [System.Serializable]
    public class EnemyPrefab
    {
        public GameObject prefab;
        [Range(0, 15)] public int spawnChance;
        public Vector3 rotationEuler = Vector3.zero;
    }

    public List<EnemyPrefab> enemyPrefabs;
    private List<GameObject> enemyPool;
    private const float spawnInterval = 1.5f;

    private void Awake()
    {
        InitializeEnemyPool();
        GameManager.OnGameStateChange += HandleGameStateChange;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChange -= HandleGameStateChange;
    }

    private void InitializeEnemyPool()
    {
        enemyPool = new List<GameObject>();
        foreach (var enemy in enemyPrefabs)
        {
            for (int i = 0; i < enemy.spawnChance; i++)
            {
                GameObject obj = Instantiate(enemy.prefab);
                obj.transform.rotation = Quaternion.Euler(enemy.rotationEuler);
                obj.SetActive(false);
                enemyPool.Add(obj);
            }
        }
    }

    private void HandleGameStateChange(GameState newState)
    {
        if (newState == GameState.Play)
        {
            spawnCoroutine = StartCoroutine(SpawnEnemiesRegularly());
        }
        else
        {
            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
            }
        }
    }

    private IEnumerator SpawnEnemiesRegularly()
    {
        while (true)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    // along up/down axis by certain amount, visualizde by gizmos
    private void SpawnEnemy()
    {
        int randomIndex = Random.Range(0, enemyPool.Count);
        GameObject enemy = enemyPool[randomIndex];

        if (!enemy.activeInHierarchy)
        {
            enemy.transform.position = startPoint.position;
            enemy.SetActive(true);
            enemy.GetComponent<Enemy>().SetEndPoint(endPoint.position);
        }
    }

    private void OnDrawGizmos()
    {
        if (startPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(startPoint.position, 0.5f);
        }

        if (endPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(endPoint.position, 0.5f);
        }
    }
}
