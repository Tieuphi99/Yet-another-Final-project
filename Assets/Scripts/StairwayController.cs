using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairwayController : MonoBehaviour
{
    public bool isVerticalStairway;

    public float moveSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isVerticalStairway)
        {
            MoveVertical();
        }
        else
        {
            MoveHorizontal();
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
