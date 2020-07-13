﻿using SystemScripts;
using UnityEngine;

namespace EnemyScripts
{
    public class EnemyHead : MonoBehaviour
    {
        private EnemyController _enemyController;
        public GameObject enemy;

        private void Awake()
        {
            _enemyController = enemy.GetComponent<EnemyController>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("BigPlayer"))
            {
                GameStatusController.Score += 200;
                other.rigidbody.AddForce(new Vector2(0f, _enemyController.pushForce));
                _enemyController.speed = 0;
                _enemyController.Die();
            }
        }
    }
}
