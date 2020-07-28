using System;
using System.Collections;
using System.Collections.Generic;
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
        Debug.Log(other.gameObject.tag);
        if (other.gameObject.CompareTag("Ground"))
        {
            _fireBallRb.AddForce(new Vector2(0, 180));
        }

        if (!other.gameObject.CompareTag("Ground"))
        {
            _fireBallAnim.SetTrigger(DestroyT);
            StartCoroutine(Destroy());
        }
    }
    
    private IEnumerator Destroy()
    {
        yield return new WaitForSeconds(0.02f);
        Destroy(gameObject);
    }
}
