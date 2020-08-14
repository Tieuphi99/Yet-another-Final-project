using System.Collections;
using SystemScripts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlayerScripts
{
    public class PlayerController : MonoBehaviour
    {
        public float speed = 410f;
        public float slideDownSpeed = 410f;
        public float jumpForce = 795f;
        private float _flagPos;
        private float _startInvincible;
        private float _invincibleTime;
        [Range(0, 1)] public float smoothTime = 0.6f;
        public bool isDead;
        private bool _isOnGround;
        private bool _isEatable;
        private bool _isFinish;
        private bool _isNotHugPole;
        private bool _isFacingRight;
        private bool _isGoingDownPipeAble;
        private bool _isAboveSpecialPipe;
        public bool isWalkingToCastle;
        public bool isInCastle;
        public bool isStopTime;
        public bool isInvulnerable;
        public bool isInvincible;
        private Vector3 _velocity;

        [Header("GameObject Settings")] public GameObject playerSprite;
        public GameObject bigPlayer;
        public GameObject bigPlayerCollider;
        public GameObject smallPlayer;
        public GameObject smallPlayerCollider;
        public GameObject playerCol;
        public GameObject fireBallPrefab;
        public Transform fireBallParent;
        private Animator _playerAnim;
        private Rigidbody2D _playerRb;
        private AudioSource _playerAudio;

        [Header("AudioClip Settings")] public AudioClip jumpSound;
        public AudioClip jumpBigSound;
        public AudioClip flagPoleSound;
        public AudioClip pipeSound;
        public AudioClip dieSound;
        public AudioClip oneUpSound;
        public AudioClip turnBigSound;
        public AudioClip coinSound;
        public AudioClip kickSound;
        public AudioClip endGameSound;
        public AudioClip fireballSound;

        private static readonly int IdleB = Animator.StringToHash("Idle_b");
        private static readonly int WalkB = Animator.StringToHash("Walk_b");
        private static readonly int RunB = Animator.StringToHash("Run_b");
        private static readonly int SpeedF = Animator.StringToHash("Speed_f");
        private static readonly int JumpTrig = Animator.StringToHash("Jump_trig");
        private static readonly int DieB = Animator.StringToHash("Die_b");
        private static readonly int BigB = Animator.StringToHash("Big_b");
        private static readonly int HugB = Animator.StringToHash("Hug_b");
        private static readonly int UltimateB = Animator.StringToHash("Ultimate_b");
        private static readonly int UltimateDurationF = Animator.StringToHash("UltimateDuration_f");
        private static readonly int CrouchB = Animator.StringToHash("Crouch_b");
        private static readonly int VulnerableB = Animator.StringToHash("Vulnerable_b");
        private static readonly int FireB = Animator.StringToHash("Fire_b");

        void Awake()
        {
            _isFacingRight = true;
            isInvulnerable = false;
            if (GameStatusController.PlayerTag != null)
            {
                tag = GameStatusController.PlayerTag;
            }

            _playerAudio = GetComponent<AudioSource>();
            _velocity = Vector3.zero;
            _playerAnim = GetComponent<Animator>();
            _playerRb = GetComponent<Rigidbody2D>();
            _isFinish = false;
            _isOnGround = true;
            isInCastle = false;
        }

        private void Update()
        {
            if (!isDead && !_isFinish && !GameStatusController.IsGameFinish)
            {
                if (Input.GetKeyDown(KeyCode.Space) && GameStatusController.IsFirePlayer)
                {
                    Instantiate(fireBallPrefab, fireBallParent.position, fireBallParent.rotation);
                    _playerAudio.PlayOneShot(fireballSound);
                }

                if (Input.GetKeyDown(KeyCode.A) && _isOnGround)
                {
                    _playerAudio.PlayOneShot(GameStatusController.IsBigPlayer ? jumpSound : jumpBigSound);
                    _isOnGround = false;
                    _playerAnim.SetTrigger(JumpTrig);
                    _playerRb.AddForce(new Vector2(0f, jumpForce));
                    _playerAnim.SetBool(IdleB, false);
                    _playerAnim.SetBool(WalkB, false);
                    _playerAnim.SetBool(RunB, false);
                }

                DenyMidAirJump();

                if (Input.GetKeyDown(KeyCode.S))
                {
                    speed = 600;
                    jumpForce = 1160;
                }

                else if (Input.GetKeyUp(KeyCode.S))
                {
                    speed = 410;
                    jumpForce = 1030;
                }

                if (Input.GetKeyDown(KeyCode.DownArrow) && _isAboveSpecialPipe)
                {
                    _isAboveSpecialPipe = false;
                    _playerAudio.PlayOneShot(pipeSound);
                    _isGoingDownPipeAble = true;
                    _playerRb.velocity = Vector2.zero;
                    StartCoroutine(StopGoingDownPipe());
                }

                if (Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.DownArrow))
                {
                    if (!_isFacingRight)
                    {
                        transform.Rotate(0, 180, 0);
                        _isFacingRight = !_isFacingRight;
                    }
                }

                if (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.DownArrow))
                {
                    if (_isFacingRight)
                    {
                        transform.Rotate(0, 180, 0);
                        _isFacingRight = !_isFacingRight;
                    }
                }

                if (_isGoingDownPipeAble)
                {
                    if (CompareTag("Player"))
                    {
                        smallPlayerCollider.SetActive(false);
                    }
                    else if (CompareTag("BigPlayer"))
                    {
                        bigPlayerCollider.SetActive(false);
                    }

                    _playerRb.isKinematic = true;
                    transform.Translate(slideDownSpeed / 2.5f * Time.deltaTime * Vector3.down);
                }
            }
        }

        private void FixedUpdate()
        {
            if (GameStatusController.IsGameFinish)
            {
                transform.Translate(slideDownSpeed / 1.25f * Time.deltaTime * Vector3.right);
                if (Input.GetKeyDown(KeyCode.Backspace))
                {
                    GameStatusController.IsGameFinish = false;
                    GameStatusController.IsShowMessage = false;
                    SceneManager.LoadScene(0);
                }
            }

            if (Mathf.RoundToInt(transform.position.x) == 285)
            {
                GameStatusController.IsBossBattle = true;
            }

            isDead = GameStatusController.IsDead;
            if (isInvincible)
            {
                _invincibleTime = Time.time - _startInvincible;
                _playerAnim.SetFloat(UltimateDurationF, _invincibleTime);
                Physics2D.IgnoreLayerCollision(8, 9, true);
                if (Time.time - _startInvincible > 10)
                {
                    StartCoroutine(BeNormal());
                }
            }

            if (isInvulnerable)
            {
                Physics2D.IgnoreLayerCollision(8, 9, true);
                StartCoroutine(BeVulnerable());
            }

            if (isDead)
            {
                Die();
            }
            else if (!isDead && !_isFinish && !GameStatusController.IsGameFinish)
            {
                _playerAnim.SetBool(BigB, GameStatusController.IsBigPlayer);
                _playerAnim.SetBool(FireB, GameStatusController.IsFirePlayer);
                ChangeAnim();
                MovePlayer();
                GetPlayerSpeed();
            }

            if (_isFinish)
            {
                if (transform.position.y > 1.5f)
                {
                    _playerAnim.SetBool(HugB, true);
                    _playerAnim.SetFloat(SpeedF, 0);
                    transform.Translate(slideDownSpeed * Time.deltaTime * Vector3.down);
                }
                else
                {
                    if (transform.position.x < _flagPos + 0.8f)
                    {
                        _playerAnim.SetBool(HugB, false);
                        transform.localScale = new Vector3(-1, 1, 1);
                        transform.position = new Vector3(_flagPos + 0.8f, transform.position.y);
                    }

                    _playerRb.isKinematic = false;
                    StartCoroutine(HugPole());
                    if (_isNotHugPole)
                    {
                        transform.localScale = Vector3.one;
                        _playerAnim.SetFloat(SpeedF, 3f);
                        transform.Translate(slideDownSpeed * Time.deltaTime * Vector3.right);
                    }
                }
            }
        }

        private void MovePlayer()
        {
            if (!Input.GetKey(KeyCode.DownArrow) && !_isGoingDownPipeAble)
            {
                var horizontalInput = Input.GetAxisRaw("Horizontal");
                Vector2 playerVelocity = _playerRb.velocity;
                Vector3 targetVelocity = new Vector2(horizontalInput * speed * Time.fixedDeltaTime, playerVelocity.y);
                _playerRb.velocity = Vector3.SmoothDamp(playerVelocity, targetVelocity, ref _velocity, smoothTime);
            }

            if (Input.GetKey(KeyCode.DownArrow) && (CompareTag("BigPlayer") || CompareTag("UltimateBigPlayer")) &&
                !_isAboveSpecialPipe)
            {
                _playerAnim.SetBool(CrouchB, true);
                smallPlayerCollider.SetActive(true);
                bigPlayerCollider.SetActive(false);
            }
            else if (Input.GetKeyUp(KeyCode.DownArrow) &&
                     (CompareTag("BigPlayer") || CompareTag("UltimateBigPlayer")) && !_isAboveSpecialPipe)
            {
                _playerAnim.SetBool(CrouchB, false);
                smallPlayerCollider.SetActive(false);
                bigPlayerCollider.SetActive(true);
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Pipe") ||
                other.gameObject.CompareTag("Brick") || other.gameObject.CompareTag("Stone") ||
                other.gameObject.CompareTag("SpecialPipe"))
            {
                _isOnGround = true;
                _playerAnim.SetBool(IdleB, true);
                _playerAnim.SetBool(WalkB, true);
                _playerAnim.SetBool(RunB, true);
            }

            if (other.gameObject.CompareTag("PowerBrick"))
            {
                _isEatable = false;
            }

            if (other.gameObject.CompareTag("Pole"))
            {
                _playerAudio.PlayOneShot(flagPoleSound);
                _flagPos = other.gameObject.transform.position.x;
                _isFinish = true;
                _playerRb.velocity = Vector2.zero;
                _playerRb.isKinematic = true;
                isWalkingToCastle = true;
                isStopTime = true;
                StartCoroutine(PlayStageClearSound());
            }

            if (other.gameObject.CompareTag("Castle"))
            {
                isInCastle = true;
                isWalkingToCastle = false;
                playerSprite.SetActive(false);
            }

            if (other.gameObject.CompareTag("DeathAbyss"))
            {
                _playerAudio.PlayOneShot(dieSound);
                GameStatusController.Live -= 1;
                GameStatusController.IsBigPlayer = false;
                GameStatusController.IsFirePlayer = false;
                GameStatusController.PlayerTag = "Player";
                GameStatusController.IsDead = true;
                _playerRb.isKinematic = true;
                _playerRb.velocity = Vector2.zero;
            }

            if (other.gameObject.CompareTag("1UpMushroom") && _isEatable)
            {
                _playerAudio.PlayOneShot(oneUpSound);
                GameStatusController.Live += 1;
                _isEatable = false;
            }

            if (other.gameObject.CompareTag("BigMushroom") && _isEatable)
            {
                _playerAudio.PlayOneShot(turnBigSound);
                TurnIntoBigPlayer();
                _isEatable = false;
            }

            if (other.gameObject.CompareTag("UltimateStar") && _isEatable)
            {
                _playerAudio.PlayOneShot(turnBigSound);
                if (CompareTag("Player"))
                {
                    tag = "UltimatePlayer";
                }
                else
                {
                    tag = "UltimateBigPlayer";
                }

                isInvincible = true;
                _playerAnim.SetBool(UltimateB, isInvincible);
                _startInvincible = Time.time;
                _isEatable = false;
            }

            if (other.gameObject.CompareTag("FireFlower") && (CompareTag("Player") || CompareTag("UltimatePlayer")) &&
                _isEatable)
            {
                _playerAudio.PlayOneShot(turnBigSound);
                TurnIntoBigPlayer();
                _isEatable = false;
            }

            if (other.gameObject.CompareTag("FireFlower") && (CompareTag("BigPlayer") || CompareTag("UltimateBigPlayer")) &&
                _isEatable)
            {
                _playerAudio.PlayOneShot(turnBigSound);
                GameStatusController.IsFirePlayer = true;
                _isEatable = false;
            }

            if (other.gameObject.CompareTag("SpecialPipe"))
            {
                _isAboveSpecialPipe = true;
            }

            if (other.gameObject.CompareTag("KoopaShell"))
            {
                _playerAudio.PlayOneShot(kickSound);
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("PowerBrick"))
            {
                _isEatable = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Princess"))
            {
                slideDownSpeed = 0;
                GameStatusController.IsShowMessage = true;
                _playerAnim.SetFloat(SpeedF, 0);
            }

            if (other.gameObject.CompareTag("Axe"))
            {
                _playerAudio.PlayOneShot(endGameSound);
                Destroy(other.gameObject);
                GameStatusController.IsBossBattle = false;
                GameStatusController.IsGameFinish = true;
                _playerAnim.SetFloat(SpeedF, 3f);
                _playerRb.velocity = Vector2.zero;
            }

            if (other.gameObject.CompareTag("Coin"))
            {
                _playerAudio.PlayOneShot(coinSound);
            }
        }

        private void TurnIntoBigPlayer()
        {
            if (CompareTag("Player"))
            {
                GameStatusController.PlayerTag = "BigPlayer";
                tag = GameStatusController.PlayerTag;
            }
            else
            {
                tag = "UltimateBigPlayer";
            }

            GameStatusController.IsBigPlayer = true;
            ChangeAnim();
        }

        private void Die()
        {
            _playerAnim.SetBool(DieB, isDead);
            GameStatusController.IsDead = true;
            StartCoroutine(DieAnim());
            StartCoroutine(LoadingScene());
        }

        private void GetPlayerSpeed()
        {
            _playerAnim.SetFloat(SpeedF, Mathf.Abs(_playerRb.velocity.x));
        }

        public void ChangeAnim()
        {
            bigPlayer.SetActive(GameStatusController.IsBigPlayer);
            bigPlayerCollider.SetActive(GameStatusController.IsBigPlayer);
            smallPlayer.SetActive(!GameStatusController.IsBigPlayer);
            smallPlayerCollider.SetActive(!GameStatusController.IsBigPlayer);
        }

        private void DenyMidAirJump()
        {
            if (_playerRb.velocity.y > 0 || _playerRb.velocity.y < 0)
            {
                _isOnGround = false;
                _playerAnim.SetBool(IdleB, false);
                _playerAnim.SetBool(WalkB, false);
                _playerAnim.SetBool(RunB, false);
            }
            else
            {
                _isOnGround = true;
                _playerAnim.SetBool(IdleB, true);
                _playerAnim.SetBool(WalkB, true);
                _playerAnim.SetBool(RunB, true);
            }
        }

        private IEnumerator SetBoolEatable()
        {
            yield return new WaitForSeconds(0.5f);
            _isEatable = true;
        }

        private IEnumerator HugPole()
        {
            yield return new WaitForSeconds(1.5f);
            _isNotHugPole = true;
        }

        private IEnumerator DieAnim()
        {
            yield return new WaitForSeconds(1);
            playerCol.SetActive(false);
            _playerRb.isKinematic = false;
        }

        private static IEnumerator LoadingScene()
        {
            yield return new WaitForSeconds(3.5f);
            SceneManager.LoadScene(1);
        }

        private IEnumerator PlayStageClearSound()
        {
            yield return new WaitForSeconds(1.5f);
            GameStatusController.IsStageClear = true;
        }

        private IEnumerator BeVulnerable()
        {
            yield return new WaitForSeconds(2);
            _playerAnim.SetBool(VulnerableB, true);
            Physics2D.IgnoreLayerCollision(8, 9, false);
            isInvulnerable = false;
        }

        private IEnumerator BeNormal()
        {
            yield return new WaitForSeconds(2);
            tag = GameStatusController.PlayerTag;
            isInvincible = false;
            _playerAnim.SetBool(UltimateB, isInvincible);
            Physics2D.IgnoreLayerCollision(8, 9, false);
        }

        private IEnumerator StopGoingDownPipe()
        {
            yield return new WaitForSeconds(1.5f);
            _isGoingDownPipeAble = false;
            SceneManager.LoadScene(GameStatusController.CurrentLevel);
            GameStatusController.CurrentLevel += 1;
        }
    }
}
