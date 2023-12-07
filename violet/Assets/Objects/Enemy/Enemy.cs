using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float moveSpeed = 6f;
    private Vector3 originalScale;
    private Vector3 endPoint;
    private bool isActive;
    private float closeToEndpointThreshold = 0.5f;

    void Awake()
    {
        originalScale = transform.localScale;
    }

    public void SetEndPoint(Vector3 newEndPoint)
    {
        endPoint = newEndPoint;
    }

    void OnEnable()
    {
        isActive = true;
        transform.localScale = Vector3.zero;
        StartCoroutine(ScaleUp());
        StartCoroutine(MoveTowardsEndPoint());
        ScoreBoard.OnSpeedAdjustment += HandleSpeedAdjustment;
    }

    void OnDisable()
    {
        isActive = false;
        ScoreBoard.OnSpeedAdjustment -= HandleSpeedAdjustment;
    }

    private IEnumerator ScaleUp()
    {
        float scaleTime = 1f; // Time to scale up
        float startTime = Time.time;

        while (Time.time - startTime < scaleTime)
        {
            transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, (Time.time - startTime) / scaleTime);
            yield return null;
        }
    }

    private void HandleSpeedAdjustment(float newSpeed)
    {
        moveSpeed = newSpeed;
    }

    private IEnumerator MoveTowardsEndPoint()
    {
        while (isActive && Vector3.Distance(transform.position, endPoint) > closeToEndpointThreshold)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPoint, moveSpeed * Time.deltaTime);
            yield return null;
        }

        if (isActive)
        {
            yield return StartCoroutine(ScaleDown());
            gameObject.SetActive(false);
        }
    }

    private IEnumerator ScaleDown()
    {
        float scaleTime = 1f; // Time to scale down
        Vector3 targetScale = Vector3.zero; // Scale down to 0
        float startTime = Time.time;

        while (Time.time - startTime < scaleTime)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, (Time.time - startTime) / scaleTime);
            yield return null;
        }
    }
}
