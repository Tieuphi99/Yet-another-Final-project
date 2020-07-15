using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace EnemyScripts
{
    public class EnemyController : MonoBehaviour
    {
        public int speed = 2;
        public float pushForce = 400;
        public bool isTouchByPlayer;

        private Animator _enemyAnim;
        public List<Collider2D> deadDisableCollider;
        public Collider2D deadEnableCollider;

        private static readonly int DieB = Animator.StringToHash("Die_b");

        private void Awake()
        {
            _enemyAnim = GetComponent<Animator>();
        }

        void Update()
        {
            Move();
        }

        void Move()
        {
            transform.Translate(speed * Time.deltaTime * Vector3.left);
        }

        public void Die()
        {
            isTouchByPlayer = true;
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
            Vector3 localScale = transform.localScale;
            if (!other.gameObject.CompareTag("Player") && !other.gameObject.CompareTag("Ground") &&
                !other.gameObject.CompareTag("Brick") && !other.gameObject.CompareTag("ScreenBorder") &&
                !other.gameObject.CompareTag("BigPlayer"))
            {
                speed = -speed;
                Move();
            }

            if (CompareTag("Koopa") && speed < 0)
            {
                localScale.x *= -1f;
            }
            else
            {
                localScale.x *= -1f;
            }
        }

        IEnumerator Destroy()
        {
            yield return new WaitForSeconds(0.3f);
            Destroy(gameObject);
        }
    }
}