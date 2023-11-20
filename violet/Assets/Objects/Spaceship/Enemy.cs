using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float moveSpeed = 5f;
    private float lifetime = 8f;
    public string poolTag;

    void Start()
    {
        StartCoroutine(MoveForward());
        StartCoroutine(ScaleDownAndDestroy());
    }

    private IEnumerator MoveForward()
    {
        float startTime = Time.time;
        while (Time.time - startTime < lifetime)
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Comet"))
        {
            ReturnToPool();
        }
    }

    private IEnumerator ScaleDownAndDestroy()
    {
        yield return new WaitForSeconds(lifetime);

        // Scale down logic
        float scaleTime = 1f; // Time to scale down
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = Vector3.zero; // Scale down to 0
        float startTime = Time.time;

        while (Time.time - startTime < scaleTime)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, (Time.time - startTime) / scaleTime);
            yield return null;
        }

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        PoolManager.Instance.ReturnToPool(poolTag, gameObject);
    }
}
