using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRotator : MonoBehaviour
{
    private Vector2 mouse_Position;
    [SerializeField] private float rotate_Speed;

    [SerializeField] private Transform tf_FollowPoint;
    //Mouse rotation related
    private float rotX; // around x

    public void OnLook(InputValue value)
    {
        mouse_Position = value.Get<Vector2>();
    }
    private void Update()
    {
        RotateTowards();
    }
    void RotateTowards()
    {
        rotX = mouse_Position.x * rotate_Speed * Mathf.Deg2Rad * Time.deltaTime * rotate_Speed;
        tf_FollowPoint.Rotate(Vector3.up, rotX);
    }
}
