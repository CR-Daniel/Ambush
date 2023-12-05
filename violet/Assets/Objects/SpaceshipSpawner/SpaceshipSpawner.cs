using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipSpawner : MonoBehaviour
{
    private List<Transform> spawnPoints;
    private Coroutine spawnCoroutine;
    public int avoidLastNSpawns = 3;
    private Queue<int> recentSpawnIndices;


    private void Awake() {
        GameManager.OnGameStateChange += HandleGameStateChange;
        recentSpawnIndices = new Queue<int>();

        spawnPoints = new List<Transform>();
        foreach (Transform child in transform)
        {
            spawnPoints.Add(child);
        }
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
            yield return new WaitForSeconds(1.5f);
        }
    }

    private void SpawnRandomEnemy()
    {
        int spawnIndex = Random.Range(0, spawnPoints.Count);

        // Ensure we don't spawn at the same point as the last N spawns
        while (recentSpawnIndices.Contains(spawnIndex))
        {
            spawnIndex = Random.Range(0, spawnPoints.Count);
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
}
