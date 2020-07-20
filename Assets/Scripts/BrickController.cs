using System;
using System.Collections;
using SystemScripts;
using UnityEngine;

public class BrickController : MonoBehaviour
{
    public bool isTouchByPlayer;
    public bool isSpecialBrick;
    public int specialBrickHealth;
    public BoxCollider2D disableCollider;
    public GameObject breakBrickPieces;
    public GameObject animationSprite;
    private Animator _brickAnim;
    private static readonly int TouchB = Animator.StringToHash("Touch_b");
    private static readonly int TouchT = Animator.StringToHash("Touch_t");
    private static readonly int SpecialB = Animator.StringToHash("Special_b");
    private static readonly int FinalHitB = Animator.StringToHash("FinalHit_b");

    private void Awake()
    {
        _brickAnim = GetComponent<Animator>();
    }

    private void Update()
    {
        _brickAnim.SetBool(SpecialB, isSpecialBrick);
        if (specialBrickHealth == 0)
        {
            _brickAnim.SetBool(FinalHitB, true);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && !isSpecialBrick)
        {
            isTouchByPlayer = true;
            _brickAnim.SetBool(TouchB, isTouchByPlayer);
        }

        else if (other.gameObject.CompareTag("BigPlayer") && !isSpecialBrick)
        {
            disableCollider.enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
            breakBrickPieces.SetActive(true);
            animationSprite.SetActive(false);
            _brickAnim.SetTrigger(TouchT);
            StartCoroutine(Destroy());
        }

        if ((other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("BigPlayer")) && isSpecialBrick)
        {
            if (specialBrickHealth > 0)
            {
                specialBrickHealth -= 1;
                GameStatusController.CollectedCoin += 1;
                GameStatusController.Score += 200;
                GameStatusController.IsEnemyDieOrCoinEat = true;
                isTouchByPlayer = true;
                _brickAnim.SetBool(TouchB, isTouchByPlayer);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("BigPlayer"))
        {
            isTouchByPlayer = false;
            _brickAnim.SetBool(TouchB, isTouchByPlayer);
        }
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}