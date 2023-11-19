using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CometSpawner : MonoBehaviour
{
    public GameObject cometPrefab;
    public Transform initialSpawnPoint;
    public Transform leftReferencePoint;
    public Transform rightReferencePoint;
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
        if (newState == GameState.Play)
        {
            SpawnCometAtReferencePoint();
        }
    }

    private void HandleCometHit(CometHitEventArgs args)
    {
        if (isFirstHit)
        {
            isFirstHit = false;
            GameManager.Instance.UpdateGameState(GameState.Play);
        }
        else
        {
            // Allow spawning the next comet
            canSpawnNextComet = true;
            StartCoroutine(SpawnCometAfterDelay());
        }
    }

    private void SpawnComet(Vector3 position)
    {
        GameObject comet = Instantiate(cometPrefab, position, Quaternion.identity);
        Comet cometScript = comet.GetComponent<Comet>();
        if (cometScript != null) {
            cometScript.OnHit += HandleCometHit;
            FindObjectOfType<ScoreBoard>().RegisterComet(cometScript);
        }
    }

    private IEnumerator SpawnCometAfterDelay()
    {
        // Wait for 1 second
        yield return new WaitForSeconds(1f);
        
        // Spawn the next comet
        canSpawnNextComet = true;
        SpawnCometAtReferencePoint();
    }

    private void SpawnCometAtReferencePoint()
    {
        if (canSpawnNextComet)
        {
            canSpawnNextComet = false;
            Transform referencePoint = Random.Range(0, 2) == 0 ? leftReferencePoint : rightReferencePoint;
            SpawnComet(referencePoint.position);
        }
    }
    
    private void OnDrawGizmos()
    {
        // Draw initial spawn point location
        if (initialSpawnPoint != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(initialSpawnPoint.position, 0.1f);
        }
        
        // Draw left and right reference points in the editor
        Gizmos.color = Color.blue;
        if (leftReferencePoint != null) {
            Gizmos.DrawSphere(leftReferencePoint.position, 0.1f);
        }
        if (rightReferencePoint != null) {
            Gizmos.DrawSphere(rightReferencePoint.position, 0.1f);
        }
    }
}
