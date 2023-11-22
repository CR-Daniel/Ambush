using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Comet : MonoBehaviour
{
    public GameEvent onCometHitRacket;
    public GameEvent onCometHitEnemy;
    private bool isHitByRacket = false;
    public float lifespan = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("String") && !isHitByRacket)
        {
            isHitByRacket = true;
            // enable fire vfx?
            // haptic feedback
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
            // instance explosion
            // haptic feedback
            // sfx
            onCometHitEnemy.Raise();
            Destroy(gameObject);
        }
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
