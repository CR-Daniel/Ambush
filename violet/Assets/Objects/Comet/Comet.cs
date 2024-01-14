using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Comet : MonoBehaviour
{
    public GameEvent onCometHitRacket;
    public GameObject vfxPrefab;
    public AudioClip hitRacketSound;
    public AudioClip hitEnemySound;
    private AudioSource audioSource;
    private bool isHitByRacket = false;
    public float lifespan = 10f;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isHitByRacket)
        {
            isHitByRacket = true;

            AdjustCometVelocity(other);

            // Calculate haptic feedback intensity based on hit speed
            Rigidbody cometRigidbody = GetComponent<Rigidbody>();
            float hitIntensity = Mathf.Clamp(cometRigidbody.velocity.magnitude / 40f, 0f, 1f);

            // dynamic sfx
            PlaySoundEffect(hitRacketSound, Mathf.Clamp(cometRigidbody.velocity.magnitude / 40f, 0.01f, 0.1f));

            if (other.CompareTag("LeftRacket"))
            {
                SendHapticFeedback(XRNode.LeftHand, hitIntensity, 0.3f);
            }
            else if (other.CompareTag("RightRacket"))
            {
                SendHapticFeedback(XRNode.RightHand, hitIntensity, 0.3f);
            }

            Destroy(gameObject, lifespan);
            onCometHitRacket.Raise(this, null);
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

            PlaySoundEffect(hitEnemySound, 0.75f);
            GetComponent<Collider>().enabled = false;
            GetComponent<MeshRenderer>().enabled = false;
            Destroy(gameObject, 0.5f);
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
            cometRigidbody.velocity = averageVelocity * 2.5f; 
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

    private void PlaySoundEffect(AudioClip clip, float volume)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f); // Vary pitch slightly
            audioSource.volume = volume; // Set volume based on hit intensity
            audioSource.PlayOneShot(clip);
        }
    }
}
