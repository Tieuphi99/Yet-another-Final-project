using SystemScripts;
using PlayerScripts;
using UnityEngine;

namespace EnemyScripts
{
    public class KoopaShell : MonoBehaviour
    {
        public GameObject koopa;
        private bool _isMoveRight;
        private bool _isMove;
        private bool _isPlayerKillable;
        public float speed;

        private AudioSource _enemyAudio;

        public AudioClip hitPlayerSound;
        public AudioClip kickSound;
        public AudioClip turnSmallPlayerSound;

        private void Awake()
        {
            _enemyAudio = GetComponent<AudioSource>();
        }

        // Update is called once per frame
        void Update()
        {
            if (_isMove)
            {
                Move();
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag("Player"))
            {
                _enemyAudio.PlayOneShot(kickSound);
            }

            if (!_isPlayerKillable)
            {
                if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("BigPlayer"))
                {
                    koopa.tag = "KoopaShell";
                    Vector3 relative = transform.InverseTransformPoint(other.transform.position);
                    float angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
                    _isMove = true;
                    if (other.gameObject.CompareTag("Player"))
                    {
                        if (angle > 0)
                        {
                            _isMoveRight = false;
                        }
                        else
                        {
                            _isMoveRight = true;
                        }
                    }
                    else if (other.gameObject.CompareTag("BigPlayer"))
                    {
                        if (angle < 0)
                        {
                            _isMoveRight = false;
                        }
                        else
                        {
                            _isMoveRight = true;
                        }
                    }

                    _isPlayerKillable = true;
                }
            }
            else
            {
                PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
                if (other.gameObject.CompareTag("Player"))
                {
                    speed = 0;
                    // StartCoroutine(Die(other.gameObject));
                    if (!playerController.isInvulnerable)
                    {
                        _enemyAudio.PlayOneShot(hitPlayerSound);
                        GameStatusController.IsDead = true;
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
        }

        private void Move()
        {
            if (_isMoveRight)
            {
                koopa.transform.Translate(speed * Time.deltaTime * Vector3.right);
            }
            else
            {
                koopa.transform.Translate(-speed * Time.deltaTime * Vector3.right);
            }
        }
    }
}