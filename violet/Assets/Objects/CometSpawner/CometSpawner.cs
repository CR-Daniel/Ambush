using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CometSpawner : MonoBehaviour
{
    public GameObject cometPrefab;
    public Transform initialSpawnPoint;
    public Transform leftBoxCenter;
    public Transform rightBoxCenter;
    public Vector3 boxSize;
    private bool isFirstHit = true;
    private bool canSpawnNextComet = true;

    private void Awake() { GameManager.OnGameStateChange += HandleGameStateChange; }
    private void OnDestroy() { GameManager.OnGameStateChange -= HandleGameStateChange; }

    private void Start()
    {
        if (initialSpawnPoint != null) {
            SpawnComet(initialSpawnPoint.position);
        }
    }

    private void HandleGameStateChange(GameState newState)
    {
        if (newState == GameState.Gameplay)
        {
            SpawnCometInRandomBox();
        }
    }

    private void HandleCometHit(CometHitEventArgs args)
    {
        if (isFirstHit)
        {
            isFirstHit = false;
            GameManager.Instance.UpdateGameState(GameState.FadeOut);
        }
        else
        {
            // Allow spawning the next comet
            canSpawnNextComet = true;
            SpawnCometInRandomBox();
        }
    }

    private void SpawnComet(Vector3 position)
    {
        GameObject comet = Instantiate(cometPrefab, position, Quaternion.identity);
        Comet cometScript = comet.GetComponent<Comet>();
        if (cometScript != null) {
            cometScript.OnHit += HandleCometHit;
        }
    }

    private void SpawnCometInRandomBox()
    {
        if (canSpawnNextComet)
        {
            canSpawnNextComet = false;
            Transform boxTransform = Random.Range(0, 2) == 0 ? leftBoxCenter : rightBoxCenter;
            Vector3 spawnPosition = GetRandomPositionInBox(boxTransform);
            SpawnComet(spawnPosition);
        }
    }

    private Vector3 GetRandomPositionInBox(Transform boxTransform)
    {
        // Calculate random position within the box
        float x = Random.Range(boxTransform.position.x - boxSize.x / 2, boxTransform.position.x + boxSize.x / 2);
        float y = Random.Range(boxTransform.position.y - boxSize.y / 2, boxTransform.position.y + boxSize.y / 2);
        float z = Random.Range(boxTransform.position.z - boxSize.z / 2, boxTransform.position.z + boxSize.z / 2);
        return new Vector3(x, y, z);
    }
    
    private void OnDrawGizmos()
    {
        // Draw initial spawn point location
        if (initialSpawnPoint != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(initialSpawnPoint.position, 0.1f);
        }
        
        // Draw left and right boxes in the editor
        Gizmos.color = Color.blue;
        if (leftBoxCenter != null) {
            Gizmos.DrawWireCube(leftBoxCenter.position, boxSize);
        }
        if (rightBoxCenter != null) {
            Gizmos.DrawWireCube(rightBoxCenter.position, boxSize);
        }
    }
}
