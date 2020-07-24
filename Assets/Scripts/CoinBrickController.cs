using SystemScripts;
using UnityEngine;

public class CoinBrickController : MonoBehaviour
{
    public bool isTouchByPlayer;
    public bool isNotSpecialBrick;
    private Animator _coinBrickAnim;
    private AudioSource _coinBrickAudio;
    public AudioClip coinSound;
    private static readonly int TouchB = Animator.StringToHash("Touch_b");

    private void Awake()
    {
        _coinBrickAudio = GetComponent<AudioSource>();
        _coinBrickAnim = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if ((other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("UltimatePlayer") ||
             other.gameObject.CompareTag("BigPlayer") || other.gameObject.CompareTag("UltimateBigPlayer")) && !isTouchByPlayer)
        {
            if (isNotSpecialBrick)
            {
                _coinBrickAudio.PlayOneShot(coinSound);
                GameStatusController.Score += 200;
                GameStatusController.IsEnemyDieOrCoinEat = true;
                GameStatusController.CollectedCoin += 1;
            }

            isTouchByPlayer = true;
            _coinBrickAnim.SetBool(TouchB, isTouchByPlayer);
        }
    }
}