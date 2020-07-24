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
    private AudioSource _brickAudio;
    public AudioClip bumpSound;
    public AudioClip breakSound;
    public AudioClip coinSound;
    private static readonly int TouchB = Animator.StringToHash("Touch_b");
    private static readonly int TouchT = Animator.StringToHash("Touch_t");
    private static readonly int SpecialB = Animator.StringToHash("Special_b");
    private static readonly int FinalHitB = Animator.StringToHash("FinalHit_b");

    private void Awake()
    {
        _brickAudio = GetComponent<AudioSource>();
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
        if ((other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("UltimatePlayer")) && !isSpecialBrick)
        {
            // Vector3 relative = transform.InverseTransformPoint(other.transform.position);
            // float angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
            // Debug.Log(angle);
            // if (angle > 153 || angle < -153)
            // {
                _brickAudio.PlayOneShot(bumpSound);
                isTouchByPlayer = true;
                _brickAnim.SetBool(TouchB, isTouchByPlayer);
            // }
        }

        else if ((other.gameObject.CompareTag("BigPlayer") || other.gameObject.CompareTag("UltimateBigPlayer")) &&
                 !isSpecialBrick)
        {
            _brickAudio.PlayOneShot(breakSound);
            disableCollider.enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
            breakBrickPieces.SetActive(true);
            animationSprite.SetActive(false);
            _brickAnim.SetTrigger(TouchT);
            StartCoroutine(Destroy());
        }

        if ((other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("UltimatePlayer") ||
             other.gameObject.CompareTag("BigPlayer") || other.gameObject.CompareTag("UltimateBigPlayer")) &&
            isSpecialBrick)
        {
            if (specialBrickHealth > 0)
            {
                _brickAudio.PlayOneShot(coinSound);
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
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("UltimatePlayer") ||
            other.gameObject.CompareTag("BigPlayer") || other.gameObject.CompareTag("UltimateBigPlayer"))
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