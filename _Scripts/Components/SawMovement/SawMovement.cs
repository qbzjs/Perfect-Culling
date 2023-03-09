using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawMovement : MonoBehaviour
{
    private float leftPos = -15;
    private float rightPos = 15;
    public float speed = 20f;
    private Vector3 toPosition = Vector3.zero;

    private void Awake()
    {
        toPosition = new Vector3(rightPos, 0, 0);
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        var step = speed * Time.deltaTime;
        Vector3 current_position = transform.localPosition;
        current_position = Vector3.MoveTowards(current_position, toPosition, step);
        transform.localPosition = current_position;
        if (Vector3.Distance(transform.localPosition, toPosition) < 0.001f)
        {
            leftPos = -leftPos;
            toPosition = new Vector3(leftPos, 0, 0);
        }
    }
}
