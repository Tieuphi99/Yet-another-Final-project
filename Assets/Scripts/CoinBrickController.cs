using SystemScripts;
using UnityEngine;

public class CoinBrickController : MonoBehaviour
{
    public bool isTouchByPlayer;
    public bool isNotSpecialBrick;
    private Animator _coinBrickAnim;
    private static readonly int TouchB = Animator.StringToHash("Touch_b");

    private void Awake()
    {
        _coinBrickAnim = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("UltimatePlayer") ||
             other.gameObject.CompareTag("BigPlayer") || other.gameObject.CompareTag("UltimateBigPlayer"))
        {
            if (isNotSpecialBrick)
            {
                GameStatusController.Score += 200;
                GameStatusController.IsEnemyDieOrCoinEat = true;
                GameStatusController.CollectedCoin += 1;
            }

            isTouchByPlayer = true;
            _coinBrickAnim.SetBool(TouchB, isTouchByPlayer);
        }
    }
}