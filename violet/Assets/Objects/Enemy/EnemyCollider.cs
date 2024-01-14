using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollider : MonoBehaviour
{
    [SerializeField] private GameEvent onCometHitEnemy;
    [SerializeField] private int scoreValue = 1;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Comet"))
        {
            onCometHitEnemy.Raise(this, scoreValue);
            gameObject.transform.parent.gameObject.SetActive(false);
        }
    }
}
