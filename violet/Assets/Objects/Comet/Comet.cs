using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Comet : MonoBehaviour
{
    public GameEvent onCometHitRacket;
    public GameEvent onCometHitEnemy;
    public GameObject vfxPrefab;
    private bool isHitByRacket = false;
    public float lifespan = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("String") && !isHitByRacket)
        {
            isHitByRacket = true;
            // enable fire vfx?
            // haptic feedback (event)
            // sfx
            AdjustCometVelocity(other);
            Destroy(gameObject, lifespan);
            onCometHitRacket.Raise();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Get the position of the first contact point
            Vector3 impactPosition = collision.contacts[0].point;
            SpawnVFX(impactPosition);

            // haptic feedback (event)
            // sfx
            onCometHitEnemy.Raise();
            Destroy(gameObject);
        }
    }

    private void SpawnVFX(Vector3 position)
    {
        // Create a random rotation
        Quaternion randomRotation = Quaternion.Euler(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360));

        // Instantiate the VFX prefab at the given position with the random rotation
        Instantiate(vfxPrefab, position, randomRotation);
    }

    private void AdjustCometVelocity(Collider collider)
    {
        RacketStringFollower racketFollower = collider.GetComponent<RacketStringFollower>();
        if (racketFollower != null)
        {
            Vector3 averageVelocity = racketFollower.GetAverageVelocity();

            Rigidbody cometRigidbody = GetComponent<Rigidbody>();
            cometRigidbody.velocity = averageVelocity * 2.0f;
        }
    }
}
