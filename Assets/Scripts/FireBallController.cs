using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallController : MonoBehaviour
{
    public float speed;
    private Rigidbody2D _fireBallRb;
    
    // Start is called before the first frame update
    void Start()
    {
        _fireBallRb = GetComponent<Rigidbody2D>();
        _fireBallRb.velocity = transform.right * speed;
    }

    // Update is called once per frame
    void Update()
    {
        // transform.Translate(10 * Time.deltaTime * Vector3.right);
    }
}
