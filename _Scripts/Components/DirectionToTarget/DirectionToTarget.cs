using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionToTarget : MonoBehaviour
{
    [SerializeField] private GameObject rotationObject;

    private Vector3 targetPosition;
    private Vector3 offSet = new Vector3(0,0.02f,0);
    RaycastHit hit;
    int groundLayer = 1 << 0;

    public void SetTargetPosition(Vector3 target_position)
    {
        targetPosition = target_position;
        rotationObject.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
    }
    private void FixedUpdate()
    {
        LookAtTarget();
        AttachToGround();
    }
    private void LookAtTarget()
    {
        float angle = Vector3.SignedAngle(Vector3.forward, (transform.parent.position - targetPosition), Vector3.up);
        rotationObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
    private void AttachToGround()
    {
        if (Physics.Raycast(transform.parent.position, Vector3.down, out hit, Mathf.Infinity, groundLayer))
        {
            transform.position = hit.point + offSet;
        }
    }
}
