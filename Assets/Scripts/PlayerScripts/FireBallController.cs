using System.Collections;
using SystemScripts;
using UnityEngine;

namespace PlayerScripts
{
    public class FireBallController : MonoBehaviour
    {
        public float speed;
        private Rigidbody2D _fireBallRb;
        private Animator _fireBallAnim;
        private static readonly int DestroyT = Animator.StringToHash("Destroy_t");

        // Start is called before the first frame update
        void Start()
        {
            _fireBallAnim = GetComponent<Animator>();
            _fireBallRb = GetComponent<Rigidbody2D>();
            _fireBallRb.velocity = transform.right * speed;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Stone"))
            {
                _fireBallRb.AddForce(new Vector2(0, 180));
                if (Mathf.RoundToInt(_fireBallRb.velocity.x) == 0)
                {
                    _fireBallAnim.SetTrigger(DestroyT);
                    StartCoroutine(Destroy());
                }
            }
            else
            {
                _fireBallAnim.SetTrigger(DestroyT);
                StartCoroutine(Destroy());
            }

            if (other.gameObject.CompareTag("Piranha"))
            {
                GameStatusController.IsEnemyDieOrCoinEat = true;
                Destroy(other.gameObject);
            }
        }

        private IEnumerator Destroy()
        {
            yield return new WaitForSeconds(0.02f);
            Destroy(gameObject);
        }
    }
}