using SystemScripts;
using UnityEngine;

namespace EnemyScripts
{
    public class EnemyHead : MonoBehaviour
    {
        private GoombaController _goombaController;
        public GameObject goomba;

        private void Awake()
        {
            _goombaController = goomba.GetComponent<GoombaController>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("BigPlayer"))
            {
                GameStatusController.Score += 200;
                other.rigidbody.AddForce(new Vector2(0f, _goombaController.pushForce));
                _goombaController.speed = 0;
                _goombaController.Die();
            }
        }
    }
}
