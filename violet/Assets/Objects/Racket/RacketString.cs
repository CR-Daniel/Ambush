using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacketString : MonoBehaviour
{
    [SerializeField]
	private RacketStringFollower _racketStringFollowerPrefab;

	private void SpawnRacketStringFollower()
	{
		var follower = Instantiate(_racketStringFollowerPrefab);
		follower.transform.position = transform.position;
        follower.transform.localScale = transform.localScale;
		follower.SetFollowTarget(this);
	}

	private void Start()
	{
		SpawnRacketStringFollower();
	}
}
