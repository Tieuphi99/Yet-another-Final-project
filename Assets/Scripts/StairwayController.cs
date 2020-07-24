using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairwayController : MonoBehaviour
{
    public bool isVerticalStairway;

    public float moveSpeed;

    // Update is called once per frame
    void Update()
    {
        if (isVerticalStairway)
        {
            MoveVertical();
        }
        else
        {
            if (transform.position.x < 59)
            {
                moveSpeed = -moveSpeed;
            }
            else if (transform.position.x > 65)
            {
                moveSpeed = -moveSpeed;
            }
            MoveHorizontal();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Stone"))
        {
            moveSpeed = -moveSpeed;
        }
    }

    private void MoveVertical()
    {
        transform.Translate(Time.deltaTime * moveSpeed * Vector3.down);
    }

    private void MoveHorizontal()
    {
        transform.Translate(Time.deltaTime * moveSpeed * Vector3.left);
    }
}
