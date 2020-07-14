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

    // Start is called before the first frame update
    void Awake()
    {
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

        if (CompareTag("FireFlower"))
        {
            GetComponent<BoxCollider2D>().isTrigger = true;
        }

        if (other.gameObject.CompareTag("Stone") || other.gameObject.CompareTag("Pipe"))
        {
            speedRight = -speedRight;
        }

        if (!other.gameObject.CompareTag("Goomba") || !other.gameObject.CompareTag("Koopa")) return;
        GetComponent<BoxCollider2D>().isTrigger = true;
        GetComponent<Rigidbody2D>().isKinematic = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Goomba") || !other.gameObject.CompareTag("Koopa")) return;
        GetComponent<BoxCollider2D>().isTrigger = false;
        GetComponent<Rigidbody2D>().isKinematic = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (CompareTag("Coin") && (other.CompareTag("Player") || other.CompareTag("BigPlayer")))
        {
            GameStatusController.CollectedCoin += 1;
            GameStatusController.Score += 200;
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
        if (other.CompareTag("Player"))
        {
            isTouchByPlayer = true;
            StartCoroutine(SetBoolEatable());
        }
        else if (other.CompareTag("BigPlayer"))
        {
            isTouchByPlayer = true;
            StartCoroutine(SetBoolEatable());
        }

        if (other.CompareTag("Player") && _isEatable)
        {
            GameStatusController.Score += 1000;
            Destroy(gameObject);
        }
        else if (other.CompareTag("BigPlayer") && _isEatable)
        {
            GameStatusController.Score += 1000;
            Destroy(gameObject);
        }
    }
}