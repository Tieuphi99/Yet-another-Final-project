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
    [Range(0, 1)] public float smoothTime = 0.6f;
    public bool isDead;
    private bool _isOnGround;
    private bool _isEatable;
    private bool _isFinish;
    private bool _isNotHugPole;
    public bool isWalkingToCastle;
    public bool isInCastle;
    public bool isStopTime;
    private Vector3 _velocity;

    public GameObject playerSprite;
    public GameObject bigPlayer;
    public GameObject bigPlayerCollider;
    public GameObject smallPlayer;
    public GameObject smallPlayerCollider;
    public GameObject playerCol;

    private Animator _playerAnim;
    private Rigidbody2D _playerRb;

    private static readonly int IdleB = Animator.StringToHash("Idle_b");
    private static readonly int WalkB = Animator.StringToHash("Walk_b");
    private static readonly int RunB = Animator.StringToHash("Run_b");
    private static readonly int SpeedF = Animator.StringToHash("Speed_f");
    private static readonly int JumpTrig = Animator.StringToHash("Jump_trig");
    private static readonly int DieB = Animator.StringToHash("Die_b");
    private static readonly int BigB = Animator.StringToHash("Big_b");
    private static readonly int HugB = Animator.StringToHash("Hug_b");

    void Awake()
    {
        if (GameStatusController.PlayerTag != null)
        {
            tag = GameStatusController.PlayerTag;
        }

        _velocity = Vector3.zero;
        _playerAnim = GetComponent<Animator>();
        _playerRb = GetComponent<Rigidbody2D>();
        isDead = false;
        _isFinish = false;
        _isOnGround = true;
        isInCastle = false;
    }

    private void FixedUpdate()
    {
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
    }

    void MovePlayer()
    {
        Vector3 localScale = transform.localScale;
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        Vector2 playerVelocity = _playerRb.velocity;
        Vector3 targetVelocity = new Vector2(horizontalInput * speed * Time.fixedDeltaTime, playerVelocity.y);
        _playerRb.velocity = Vector3.SmoothDamp(playerVelocity, targetVelocity, ref _velocity, smoothTime);

        if (Input.GetKeyDown(KeyCode.Space) && _isOnGround)
        {
            _isOnGround = false;
            _playerAnim.SetTrigger(JumpTrig);
            _playerRb.AddForce(new Vector2(0f, jumpForce));
            _playerAnim.SetBool(IdleB, false);
            _playerAnim.SetBool(WalkB, false);
            _playerAnim.SetBool(RunB, false);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            if (localScale.x < 0)
            {
                localScale.x *= -1f;
            }
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (localScale.x > 0)
            {
                localScale.x *= -1f;
            }
        }

        transform.localScale = localScale;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // Debug.Log(other.gameObject.tag);
        if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Pipe") ||
            other.gameObject.CompareTag("Brick") ||
            other.gameObject.CompareTag("Stone"))
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
            _flagPos = other.gameObject.transform.position.x;
            _isFinish = true;
            _playerRb.velocity = Vector2.zero;
            _playerRb.isKinematic = true;
            isWalkingToCastle = true;
            isStopTime = true;
        }

        if (other.gameObject.CompareTag("Castle"))
        {
            StartCoroutine(NextLevel());
            isInCastle = true;
            isWalkingToCastle = false;
            playerSprite.SetActive(false);
        }

        if (!other.gameObject.CompareTag("BigMushroom") || !_isEatable) return;
        GameStatusController.PlayerTag = "BigPlayer";
        tag = GameStatusController.PlayerTag;
        GameStatusController.IsBigPlayer = true;
        ChangeAnim();
    }

    private void Die()
    {
        _playerAnim.SetBool(DieB, isDead);
        GameStatusController.IsDead = true;
        // _playerRb.velocity = Vector2.zero;
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

    private IEnumerator SetBoolEatable()
    {
        yield return new WaitForSeconds(0.5f);
        _isEatable = true;
    }

    private IEnumerator HugPole()
    {
        yield return new WaitForSeconds(1.2f);
        _isNotHugPole = true;
    }

    private IEnumerator NextLevel()
    {
        yield return new WaitForSeconds(7);
        SceneManager.LoadScene(1);
    }

    private IEnumerator DieAnim()
    {
        yield return new WaitForSeconds(1);
        playerCol.SetActive(false);
    }

    private IEnumerator LoadingScene()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(1);
    }
}