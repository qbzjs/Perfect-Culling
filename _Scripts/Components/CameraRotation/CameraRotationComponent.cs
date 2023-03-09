using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRotationComponent : MonoBehaviour
{
    [SerializeField]
    private GameObject followTransform;
    private Vector2 _lookDirection;
    private float rotationSpeed = 0.3f;
    private float xCameraRotation;
    private float yCameraRotation;
    private Vector3 defaultFollowTransformPosition;
    private Quaternion defaultFollowTransformRotation;


    private void CameraRotation()
    {
        float sqrMagnitude = _lookDirection.sqrMagnitude;
        if (sqrMagnitude >= 0.01f)
        {
            xCameraRotation += _lookDirection.x * rotationSpeed;
            yCameraRotation -= _lookDirection.y * rotationSpeed;
        }

        // clamp our rotations so our values are limited 360 degrees
        xCameraRotation = ClampAngle(xCameraRotation, float.MinValue, float.MaxValue);
        yCameraRotation = ClampAngle(yCameraRotation, -50, 50);

        // Cinemachine will follow this target
        Vector3 euler = new Vector3(yCameraRotation, xCameraRotation, 0.0f);
        var targetRotation = Quaternion.Euler(euler);
        followTransform.transform.rotation = Quaternion.Slerp(followTransform.transform.rotation, targetRotation, 0.9f);
        Vector3 eulermn = new Vector3(0.0f, 0.0f,360-xCameraRotation);
        var targetRotationmn = Quaternion.Euler(eulermn);
        Observer.Instance.Notify(ObserverKey.CameraRotation, targetRotationmn);
    }
    private void RotationToPosition(Vector3 start_position, Vector3 target_position,Quaternion start_rotation,Quaternion target_rotation,bool lock_input)
    {
        StartCoroutine(RotationCamera(start_position, target_position, start_rotation, target_rotation, lock_input));
    }
    public void RotationFromDefaultPositionToNewPosition(Vector3 target_position,Quaternion target_rotation)
    {
        defaultFollowTransformPosition = followTransform.transform.localPosition;
        defaultFollowTransformRotation = followTransform.transform.localRotation;
        RotationToPosition(defaultFollowTransformPosition, target_position, defaultFollowTransformRotation,target_rotation, true);
    }
    public void RotationToDefaultPosition()
    {
        RotationToPosition(followTransform.transform.localPosition, defaultFollowTransformPosition,followTransform.transform.localRotation,defaultFollowTransformRotation,false);
    }
    IEnumerator RotationCamera(Vector3 start_position,Vector3 target_position,Quaternion start_rotation,Quaternion target_rotation,bool lock_input)
    {
        if(lock_input)
            GameConfig.gameBlockInput = lock_input;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime;
            followTransform.transform.localRotation = Quaternion.Slerp(start_rotation, target_rotation, t / 1);
            followTransform.transform.localPosition = Vector3.Slerp(start_position, target_position, t / 1);
            yield return null;
        }
        if (!lock_input) GameConfig.gameBlockInput = lock_input;
    }
    private float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
    public void OnLook(InputValue value)
    {
        //#if !UNITY_IOS && !UNITY_ANDROID
        //        if (Cursor.lockState != CursorLockMode.Locked)
        //        {
        //            UpdateLookValue(Vector2.zero);
        //            return;
        //        }
        //#endif
        Vector2 direction = Cursor.lockState != CursorLockMode.Locked ? Vector2.zero : value.Get<Vector2>();
        UpdateLookValue(direction);
        CameraFixedUpdate();
    }

    private void UpdateLookValue(Vector2 value)
    {
        _lookDirection = value;
    }

    private void CameraFixedUpdate()
    {
        CameraRotation();
        Vector3 angles = followTransform.transform.localEulerAngles;
        angles.z = 0;


        float angle = followTransform.transform.localEulerAngles.x;

        //Clamp the Up/Down rotation
        if (angle > 180 && angle < 340)
        {
            angles.x = 340;
        }
        else if (angle < 180 && angle > 40)
        {
            angles.x = 40;
        }

        followTransform.transform.localEulerAngles = angles;
    }

}
