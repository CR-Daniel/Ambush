using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Comet : MonoBehaviour
{
    public event Action<CometHitEventArgs> OnHit;
    private bool isHitByRacket = false;
    public float lifespan = 10f;

    private void OnDestroy()
    {
        OnHit = null; // Remove all subscribers
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("String") && !isHitByRacket)
        {
            isHitByRacket = true;
            Destroy(gameObject, lifespan);
            ProcessCollision(collision);
        }
        else if (collision.gameObject.CompareTag("Shield") || collision.gameObject.CompareTag("Enemy"))
        {
            ProcessCollision(collision);
        }
    }

    private void ProcessCollision(Collision collision)
    {
        // Determine the hit object, strength, and location
        GameObject hitObject = collision.gameObject;
        float hitStrength = collision.relativeVelocity.magnitude;
        Vector3 hitLocation = collision.contacts[0].point;

        // Trigger the event with the detailed information
        CometHitEventArgs args = new CometHitEventArgs(hitObject, hitStrength, hitLocation);
        OnHit?.Invoke(args);
    }
}
