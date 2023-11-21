using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnPoint
    {
        public Transform point;
        public string enemyPoolTag;
    }

    public SpawnPoint[] spawnPoints;
    private Coroutine spawnCoroutine;


    private void Awake() { GameManager.OnGameStateChange += HandleGameStateChange; }
    private void OnDestroy() { GameManager.OnGameStateChange -= HandleGameStateChange; }

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
        while (true) // Infinite loop, will be stopped when coroutine is stopped
        {
            SpawnEnemies();
            yield return new WaitForSeconds(4f); // Wait for 5 seconds before spawning again
        }
    }


    public void SpawnEnemies()
    {
        foreach (var spawnPoint in spawnPoints)
        {
            PoolManager.Instance.SpawnFromPool(spawnPoint.enemyPoolTag, spawnPoint.point.position, spawnPoint.point.rotation);
        }
    }

    void OnDrawGizmos()
    {
        if (spawnPoints == null) return;

        foreach (var spawnPoint in spawnPoints)
        {
            if (spawnPoint.point != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(spawnPoint.point.position, 2f);
            }
        }
    }
}
