using System;
using System.Collections;
using SystemScripts;
using UnityEngine;

public class PowerUpsController : MonoBehaviour
{
    public int speedRight;
    public int speedUp;
    public bool isMoving;
    public bool isTouchByPlayer;
    private bool _isEatable;
    private float _firstYPos;

    private AudioSource _powerAudio;

    public AudioClip appearSound;

    // Start is called before the first frame update
    void Awake()
    {
        _powerAudio = GetComponent<AudioSource>();
        Physics2D.IgnoreLayerCollision(9, 10, true);
        _firstYPos = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (isTouchByPlayer && !CompareTag("Coin"))
        {
            if (transform.position.y < _firstYPos + 1)
            {
                transform.Translate(speedUp * Time.deltaTime * Vector2.up);
            }
            else if (CompareTag("BigMushroom") || CompareTag("1UpMushroom"))
            {
                isMoving = true;
                GetComponent<Rigidbody2D>().isKinematic = false;
            }
        }

        if (isMoving && (CompareTag("BigMushroom") || CompareTag("1UpMushroom")))
        {
            isTouchByPlayer = false;
            transform.Translate(speedRight * Time.deltaTime * Vector2.right);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        InteractionWithPlayer(other.gameObject);

        if (other.gameObject.CompareTag("Stone") || other.gameObject.CompareTag("Pipe") ||
            other.gameObject.CompareTag("Untagged"))
        {
            speedRight = -speedRight;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (CompareTag("Coin") && (other.CompareTag("Player") || other.CompareTag("BigPlayer") ||
                                   other.CompareTag("UltimatePlayer") || other.CompareTag("UltimateBigPlayer")))
        {
            GameStatusController.CollectedCoin += 1;
            GameStatusController.Score += 200;
            GameStatusController.IsEnemyDieOrCoinEat = true;
            Destroy(gameObject);
        }

        InteractionWithPlayer(other.gameObject);
    }

    private IEnumerator SetBoolEatable()
    {
        yield return new WaitForSeconds(1);
        _isEatable = true;
    }

    void InteractionWithPlayer(GameObject other)
    {
        if ((other.CompareTag("Player") || other.CompareTag("UltimatePlayer")) && !CompareTag("Coin"))
        {
            _powerAudio.PlayOneShot(appearSound);
            isTouchByPlayer = true;
            StartCoroutine(SetBoolEatable());
        }
        else if (other.CompareTag("BigPlayer") || other.CompareTag("UltimateBigPlayer") && !CompareTag("Coin"))
        {
            _powerAudio.PlayOneShot(appearSound);
            isTouchByPlayer = true;
            StartCoroutine(SetBoolEatable());
        }

        if ((other.CompareTag("Player") || other.CompareTag("UltimatePlayer")) && _isEatable)
        {
            GameStatusController.Score += 1000;
            GameStatusController.IsPowerUpEat = true;
            _isEatable = false;
            Destroy(gameObject);
        }
        else if ((other.CompareTag("BigPlayer") || other.CompareTag("UltimateBigPlayer")) && _isEatable)
        {
            GameStatusController.Score += 1000;
            GameStatusController.IsPowerUpEat = true;
            _isEatable = false;
            Destroy(gameObject);
        }
    }
}