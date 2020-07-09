using System.Collections;
using UnityEngine;

namespace EnemyScripts
{
    public class GoombaController : MonoBehaviour
    {
        public int speed = 2;
        public float pushForce = 400;
        public bool isTouchByPlayer;

        private Animator _goombaAnim;
        public Collider2D deadDisableCollider;
        public GameObject player;

        private static readonly int DieB = Animator.StringToHash("Die_b");

        private void Awake()
        {
            _goombaAnim = GetComponent<Animator>();
        }

        // Update is called once per frame
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
            deadDisableCollider.enabled = false;
            _goombaAnim.SetBool(DieB, true);
            StartCoroutine(Destroy());
        }

        IEnumerator Destroy()
        {
            yield return new WaitForSeconds(0.3f);
            Destroy(gameObject);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag("Player") && !other.gameObject.CompareTag("Ground") &&
                !other.gameObject.CompareTag("Brick") && !other.gameObject.CompareTag("ScreenBorder") &&
                !other.gameObject.CompareTag("BigPlayer"))
            {
                speed = -speed;
                Move();
            }
        }
    }
}