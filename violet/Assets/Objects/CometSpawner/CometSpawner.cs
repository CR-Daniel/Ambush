using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CometSpawner : MonoBehaviour
{
    public GameObject cometPrefab;
    public Transform initialSpawnPoint;
    public List<Transform> spawnPoints;
    public int numberOfComets = 1;
    private List<GameObject> activeComets = new List<GameObject>();
    private bool isFirstHit = true;
    private bool canSpawnNextComet = true;

    private void Awake() { GameManager.OnGameStateChange += HandleGameStateChange; }
    private void OnDestroy() { GameManager.OnGameStateChange -= HandleGameStateChange; }

    private void Start()
    {
        if (initialSpawnPoint != null) {
            SpawnComets(initialSpawnPoint.position, 1);
        }
    }

    private void HandleGameStateChange(GameState newState)
    {
        if (newState == GameState.Play)
        {
            SpawnCometsRandomly();
        }
    }

    public void HandleCometHitRacket(Component sender, object data)
    {
        if (isFirstHit)
        {
            isFirstHit = false;
            GameManager.Instance.UpdateGameState(GameState.Play);
        }
        else
        {
            DestroyOtherComets(sender.gameObject);
            canSpawnNextComet = true;
            StartCoroutine(SpawnCometsAfterDelay());
        }
    }

    private void SpawnComets(Vector3 position, int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject comet = Instantiate(cometPrefab, position, Quaternion.identity);
            activeComets.Add(comet);
        }
    }

    private IEnumerator SpawnCometsAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        canSpawnNextComet = true;
        SpawnCometsRandomly();
    }

    private void SpawnCometsRandomly()
    {
        if (canSpawnNextComet && spawnPoints.Count >= numberOfComets)
        {
            canSpawnNextComet = false;
            HashSet<int> selectedIndices = new HashSet<int>();

            while (selectedIndices.Count < numberOfComets)
            {
                int randomIndex = Random.Range(0, spawnPoints.Count);
                if (selectedIndices.Add(randomIndex)) // Returns false if the item was already in the set
                {
                    Transform selectedPoint = spawnPoints[randomIndex];
                    SpawnComets(selectedPoint.position, 1);
                }
            }
        }
    }

    private void DestroyOtherComets(GameObject cometToExclude)
    {
        foreach (var comet in activeComets)
        {
            if (comet != null && comet != cometToExclude)
            {
                Destroy(comet);
            }
        }
        activeComets.Clear();
    }
    
    private void OnDrawGizmos()
    {
        // Draw initial and random spawn points location
        Gizmos.color = Color.red;
        if (initialSpawnPoint != null) {
            Gizmos.DrawSphere(initialSpawnPoint.position, 0.1f);
        }
        
        Gizmos.color = Color.blue;
        foreach (var point in spawnPoints)
        {
            if (point != null) {
                Gizmos.DrawSphere(point.position, 0.1f);
            }
        }
    }
}
