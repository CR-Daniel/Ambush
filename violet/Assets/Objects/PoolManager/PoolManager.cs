using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    public GameObject[] enemyPrefabs;
    public int poolSizePerType = 5;

    private List<GameObject> enemyList;

    private void Awake()
    {
        Instance = this;

        enemyList = new List<GameObject>();

        foreach (GameObject prefab in enemyPrefabs)
        {
            for (int i = 0; i < poolSizePerType; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false);
                enemyList.Add(obj);
            }
        }

        // Shuffle the list for more randomness
        ShuffleEnemyList();
    }

    private void ShuffleEnemyList()
    {
        for (int i = 0; i < enemyList.Count; i++)
        {
            GameObject temp = enemyList[i];
            int randomIndex = Random.Range(i, enemyList.Count);
            enemyList[i] = enemyList[randomIndex];
            enemyList[randomIndex] = temp;
        }
    }

    public GameObject SpawnRandomEnemy(Vector3 position, Quaternion rotation)
    {
        if (enemyList.Count == 0)
        {
            Debug.LogWarning("No enemies available in the pool.");
            return null;
        }

        int randomIndex = Random.Range(0, enemyList.Count);
        GameObject objectToSpawn = enemyList[randomIndex];
        enemyList.RemoveAt(randomIndex);

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        return objectToSpawn;
    }

    public void ReturnToPool(GameObject objectToReturn)
    {
        objectToReturn.SetActive(false);
        enemyList.Add(objectToReturn);
    }
}
