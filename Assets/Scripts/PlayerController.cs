using System;
using System.Collections;
using SystemScripts;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public GameObject playerSprite;
    public GameObject bigPlayer;
    public GameObject bigPlayerCollider;
    public GameObject smallPlayer;
    public GameObject smallPlayerCollider;
    public GameObject playerCol;
    private Animator _playerAnim;
    private Rigidbody2D _playerRb;
    private AudioSource _playerAudio;

    public AudioClip jumpSound;
    public AudioClip jumpBigSound;
    public AudioClip stageClearSound;
    public AudioClip flagPoleSound;

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

    private void FixedUpdate()
    {
        isDead = GameStatusController.IsDead;
        if (isInvincible)
        {
            _invincibleTime = Time.time - _startInvincible;
            _playerAnim.SetFloat(UltimateDurationF, _invincibleTime);
            if (Time.time - _startInvincible > 10)
            {
                StartCoroutine(BeNormal());
            }
        }

        if (isInvulnerable)
        {
            Physics2D.IgnoreLayerCollision(8, 9);
            StartCoroutine(BeVulnerable());
        }

        if (isDead)
        {
            Die();
        }
        else if (!isDead && !_isFinish)
        {
            _playerAnim.SetBool(BigB, GameStatusController.IsBigPlayer);
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

    void MovePlayer()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        Vector2 playerVelocity = _playerRb.velocity;
        Vector3 targetVelocity = new Vector2(horizontalInput * speed * Time.fixedDeltaTime, playerVelocity.y);
        _playerRb.velocity = Vector3.SmoothDamp(playerVelocity, targetVelocity, ref _velocity, smoothTime);

        DenyMidAirJump();

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

        if (Input.GetKey(KeyCode.RightArrow))
        {
            if (!_isFacingRight)
            {
                transform.Rotate(0, 180, 0);
                _isFacingRight = !_isFacingRight;
            }
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (_isFacingRight)
            {
                transform.Rotate(0, 180, 0);
                _isFacingRight = !_isFacingRight;
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) && _isAboveSpecialPipe)
        {
            _isGoingDownPipeAble = true;
            StartCoroutine(StopGoingDownPipe());
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log(other.gameObject.tag);
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
            StartCoroutine(SetBoolEatable());
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
            _playerRb.velocity = Vector2.zero;
        }

        if (other.gameObject.CompareTag("1UpMushroom") && _isEatable)
        {
            GameStatusController.Live += 1;
            // _isEatable = false;
        }

        if (other.gameObject.CompareTag("BigMushroom") && _isEatable)
        {
            TurnIntoBigPlayer();
            // _isEatable = false;
        }

        if (other.gameObject.CompareTag("SpecialPipe"))
        {
            _isAboveSpecialPipe = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("FireFlower") && CompareTag("Player") && _isEatable)
        {
            TurnIntoBigPlayer();
            // _isEatable = false;
        }

        if (other.gameObject.CompareTag("UltimateStar") && _isEatable)
        {
            isInvincible = true;
            _playerAnim.SetBool(UltimateB, isInvincible);
            _startInvincible = Time.time;
            // _isEatable = false;
        }
    }

    private void TurnIntoBigPlayer()
    {
        GameStatusController.PlayerTag = "BigPlayer";
        tag = GameStatusController.PlayerTag;
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
    }

    private static IEnumerator LoadingScene()
    {
        yield return new WaitForSeconds(3.5f);
        SceneManager.LoadScene(1);
    }

    private IEnumerator PlayStageClearSound()
    {
        yield return new WaitForSeconds(1.5f);
        _playerAudio.PlayOneShot(stageClearSound);
    }

    private IEnumerator BeVulnerable()
    {
        yield return new WaitForSeconds(2);
        Physics2D.IgnoreLayerCollision(8, 9, false);
        isInvulnerable = false;
    }

    private IEnumerator BeNormal()
    {
        yield return new WaitForSeconds(2);
        isInvincible = false;
        _playerAnim.SetBool(UltimateB, isInvincible);
    }

    private IEnumerator StopGoingDownPipe()
    {
        yield return new WaitForSeconds(1.5f);
        _isAboveSpecialPipe = false;
        _isGoingDownPipeAble = false;
    }
}