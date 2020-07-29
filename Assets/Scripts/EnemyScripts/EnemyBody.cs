using System.Collections;
using SystemScripts;
using PlayerScripts;
using UnityEngine;

namespace EnemyScripts
{
    public class EnemyBody : MonoBehaviour
    {
        // private PlayerController _playerController;
        private EnemyController _enemyController;

        public GameObject enemy;

        private AudioSource _enemyAudio;

        public AudioClip hitPlayerSound;
        public AudioClip turnSmallPlayerSound;

        private void Awake()
        {
            _enemyAudio = GetComponent<AudioSource>();
            if (enemy != null)
            {
                _enemyController = enemy.GetComponent<EnemyController>();
            }
        }

        private void Update()
        {
            if (_enemyController != null && _enemyController.isTouchByPlayer)
            {
                GetComponent<BoxCollider2D>().offset = Vector2.zero;
                GetComponent<BoxCollider2D>().size = new Vector2(1, 0.01f);
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
            if (other.gameObject.CompareTag("Player"))
            {
                // StartCoroutine(Die(other.gameObject));
                if (!playerController.isInvulnerable)
                {
                    _enemyAudio.PlayOneShot(hitPlayerSound);
                    GameStatusController.IsDead = true;
                    GameStatusController.Live -= 1;
                    playerController.GetComponent<Rigidbody2D>().isKinematic = true;
                    playerController.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                }
                else
                {
                    Physics2D.IgnoreCollision(GetComponent<Collider2D>(),
                        playerController.smallPlayerCollider.GetComponent<Collider2D>());
                }
            }
            else if (other.gameObject.CompareTag("BigPlayer"))
            {
                _enemyAudio.PlayOneShot(turnSmallPlayerSound);
                GameStatusController.IsBigPlayer = false;
                GameStatusController.IsFirePlayer = false;
                GameStatusController.PlayerTag = "Player";
                playerController.gameObject.tag = GameStatusController.PlayerTag;
                playerController.ChangeAnim();
                playerController.isInvulnerable = true;
                // StartCoroutine(Die(other.gameObject));
            }
        }

        IEnumerator Die(GameObject playerGameObject)
        {
            yield return new WaitForSeconds(1);
            playerGameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 4000));
            playerGameObject.GetComponent<Rigidbody2D>().gravityScale = 25;
        }
    }
}