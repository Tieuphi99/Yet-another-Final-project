using SystemScripts;
using UnityEngine;

namespace EnemyScripts
{
    public class EnemyHead : MonoBehaviour
    {
        private EnemyController _enemyController;
        public GameObject enemy;
        private AudioSource _enemyAudio;
        public AudioClip hitByPlayerSound;

        private void Awake()
        {
            _enemyAudio = GetComponent<AudioSource>();
            _enemyController = enemy.GetComponent<EnemyController>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("BigPlayer"))
            {
                GameStatusController.IsEnemyDieOrCoinEat = true;
                _enemyAudio.PlayOneShot(hitByPlayerSound);
                other.rigidbody.AddForce(new Vector2(0f, _enemyController.pushForce));
                _enemyController.speed = 0;
                _enemyController.Die();
            }
        }
    }
}
