using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class RacketStringFollower : MonoBehaviour
{
    private RacketString _racketFollower;
	private Rigidbody _rigidbody;
	private Vector3 _velocity;
	private Queue<Vector3> _velocityHistory = new Queue<Vector3>();
	private int _velocityHistoryLength = 5; // Number of frames to average

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

		// Update velocity history
		_velocityHistory.Enqueue(_velocity);
		if (_velocityHistory.Count > _velocityHistoryLength)
		{
			_velocityHistory.Dequeue();
		}
	}

	public Vector3 GetAverageVelocity()
	{
		Vector3 sum = Vector3.zero;
		foreach (var vel in _velocityHistory)
		{
			sum += vel;
		}
		return sum / _velocityHistory.Count;
	}

	public void SetFollowTarget(RacketString racketFollower)
	{
		_racketFollower = racketFollower;
	}
}
