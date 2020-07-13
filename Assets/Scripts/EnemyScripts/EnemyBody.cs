using System.Collections;
using SystemScripts;
using UnityEngine;

namespace EnemyScripts
{
    public class EnemyBody : MonoBehaviour
    {
        private PlayerController _playerController;
        private EnemyController _enemyController;

        public GameObject goomba;

        private void Awake()
        {
            _enemyController = goomba.GetComponent<EnemyController>();
            _playerController = _enemyController.player.GetComponent<PlayerController>();
        }

        private void Update()
        {
            if (_enemyController.isTouchByPlayer)
            {
                GetComponent<BoxCollider2D>().offset = Vector2.zero;
                GetComponent<BoxCollider2D>().size = new Vector2(1.04f, 0.32f);
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                // StartCoroutine(Die(other.gameObject));
                GameStatusController.Live -= 1;
                _playerController.isDead = true;
            }
            else if (other.gameObject.CompareTag("BigPlayer"))
            {
                GameStatusController.IsBigPlayer = false;
                GameStatusController.PlayerTag = "Player"; 
                _playerController.gameObject.tag = GameStatusController.PlayerTag;
                _playerController.ChangeAnim();
                // StartCoroutine(Die(other.gameObject));
                // _playerController.isDead = true;
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