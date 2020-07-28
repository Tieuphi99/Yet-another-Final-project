﻿using System;
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

    private void Update()
    {
        if (Mathf.RoundToInt(_fireBallRb.velocity.x) == 0)
        {
            _fireBallAnim.SetTrigger(DestroyT);
            StartCoroutine(Destroy());
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
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