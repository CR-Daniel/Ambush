using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class RacketStringFollower : MonoBehaviour
{
    private RacketString _racketFollower;
	private Rigidbody _rigidbody;
	private Vector3 _velocity;

	[SerializeField] private float _sensitivity = 100f;
    [SerializeField] private AudioClip hitSound;
    private AudioSource audioSource;

	private void Awake()
	{
		_rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
	}

	private void FixedUpdate()
	{
		Vector3 destination = _racketFollower.transform.position;
		_rigidbody.transform.rotation = transform.rotation;

		_velocity = (destination - _rigidbody.transform.position) * _sensitivity;

		_rigidbody.velocity = _velocity;
		transform.rotation = _racketFollower.transform.rotation;
	}

	public void SetFollowTarget(RacketString racketFollower)
	{
		_racketFollower = racketFollower;
	}
}
