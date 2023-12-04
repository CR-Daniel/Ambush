using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipSpawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    private Coroutine spawnCoroutine;
    public int avoidLastNSpawns = 3;
    private Queue<int> recentSpawnIndices;


    private void Awake() {
        GameManager.OnGameStateChange += HandleGameStateChange;
        recentSpawnIndices = new Queue<int>();
    }

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
        while (true)
        {
            SpawnRandomEnemy();
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void SpawnRandomEnemy()
    {
        int spawnIndex = Random.Range(0, spawnPoints.Length);

        // Ensure we don't spawn at the same point as the last N spawns
        while (recentSpawnIndices.Contains(spawnIndex))
        {
            spawnIndex = Random.Range(0, spawnPoints.Length);
        }

        // Update recent spawn indices
        recentSpawnIndices.Enqueue(spawnIndex);
        if (recentSpawnIndices.Count > avoidLastNSpawns)
        {
            recentSpawnIndices.Dequeue();
        }

        // Spawn the enemy at the chosen point
        PoolManager.Instance.SpawnRandomEnemy(spawnPoints[spawnIndex].position, spawnPoints[spawnIndex].rotation);
    }

    void OnDrawGizmos()
    {
        if (spawnPoints == null) return;

        foreach (var point in spawnPoints)
        {
            if (point != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(point.position, 2f);
            }
        }
    }
}
