using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

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

            AdjustCometVelocity(other);

            // sfx

            // Calculate haptic feedback intensity based on hit speed
            Rigidbody cometRigidbody = GetComponent<Rigidbody>();
            Debug.Log(cometRigidbody.velocity.magnitude);
            float hitIntensity = Mathf.Clamp(cometRigidbody.velocity.magnitude / 40f, 0f, 1f);

            // Send haptic feedback to the right hand controller
            SendHapticFeedback(XRNode.RightHand, hitIntensity, 0.3f);

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

            // Send a slight rumble to both controllers
            SendHapticFeedback(XRNode.RightHand, 0.3f, 0.3f);
            SendHapticFeedback(XRNode.LeftHand, 0.3f, 0.3f);

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

    private void SendHapticFeedback(XRNode controllerNode, float intensity, float duration)
    {
        var hapticDevice = InputDevices.GetDeviceAtXRNode(controllerNode);
        if (hapticDevice.isValid)
        {
            hapticDevice.SendHapticImpulse(0, intensity, duration);
        }
    }
}
