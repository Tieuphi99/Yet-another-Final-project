using System;
using System.Collections;
using System.Collections.Generic;
using SystemScripts;
using UnityEngine;

public class FireBallController : MonoBehaviour
{
    public float speed;
    private Rigidbody2D _fireBallRb;
    private Animator _fireBallAnim;
    private static readonly int DestroyT = Animator.StringToHash("Destroy_t");

    // Start is called before the first frame update
    void Start()
    {
        _fireBallAnim = GetComponent<Animator>();
        _fireBallRb = GetComponent<Rigidbody2D>();
        _fireBallRb.velocity = transform.right * speed;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Vector3 relative = transform.InverseTransformPoint(other.transform.position);
        float angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
        Debug.Log(angle);
        if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("CastleStone"))
        {
            _fireBallRb.AddForce(new Vector2(0, 180));
        }
        else
        {
            _fireBallAnim.SetTrigger(DestroyT);
            StartCoroutine(Destroy());
        }

        if (other.gameObject.CompareTag("Piranha"))
        {
            GameStatusController.IsEnemyDieOrCoinEat = true;
            Destroy(other.gameObject);
        }
    }

    private IEnumerator Destroy()
    {
        yield return new WaitForSeconds(0.02f);
        Destroy(gameObject);
    }
}