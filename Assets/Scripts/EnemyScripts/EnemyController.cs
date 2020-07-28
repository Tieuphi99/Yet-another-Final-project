using System.Collections;
using System.Collections.Generic;
using SystemScripts;
using UnityEngine;

namespace EnemyScripts
{
    public class EnemyController : MonoBehaviour
    {
        public int speed = 2;
        public float pushForce = 500;
        public bool isTouchByPlayer;

        private Animator _enemyAnim;
        public List<Collider2D> deadDisableCollider;
        public Collider2D deadEnableCollider;

        private static readonly int DieB = Animator.StringToHash("Die_b");

        private void Awake()
        {
            _enemyAnim = GetComponent<Animator>();
        }

        private void Update()
        {
            Move();
        }

        private void Move()
        {
            transform.Translate(speed * Time.deltaTime * Vector3.left);
        }

        public void Die()
        {
            isTouchByPlayer = true;
            GameStatusController.Score += 200;
            for (var i = 0; i < deadDisableCollider.Count; i++)
            {
                deadDisableCollider[i].enabled = false;
            }

            if (deadEnableCollider != null)
            {
                deadEnableCollider.enabled = true;
            }

            _enemyAnim.SetBool(DieB, true);
            if (CompareTag("Goomba"))
            {
                StartCoroutine(Destroy());
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (CompareTag("KoopaShell"))
            {
                if (!other.gameObject.CompareTag("Player") && !other.gameObject.CompareTag("Ground") &&
                    !other.gameObject.CompareTag("Brick") && !other.gameObject.CompareTag("ScreenBorder") &&
                    !other.gameObject.CompareTag("Goomba") && !other.gameObject.CompareTag("Koopa"))
                {
                    transform.Rotate(0, 180, 0);
                }
            }
            else
            {
                if (!other.gameObject.CompareTag("Player") && !other.gameObject.CompareTag("Ground") &&
                    !other.gameObject.CompareTag("Brick") && !other.gameObject.CompareTag("ScreenBorder"))
                {
                    transform.Rotate(0, 180, 0);
                }
            }

            if (other.gameObject.CompareTag("KoopaShell") || other.gameObject.CompareTag("Fireball"))
            {
                GameStatusController.Score += 200;
                GameStatusController.IsEnemyDieOrCoinEat = true;
                Destroy(gameObject);
            }
        }

        IEnumerator Destroy()
        {
            yield return new WaitForSeconds(0.3f);
            Destroy(gameObject);
        }
    }
}