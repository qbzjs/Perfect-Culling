using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SitComponent : MonoBehaviour
{
    private EntityManager _entityManager;
    private EntityManager entityManager
    {
        get
        {
            if (_entityManager == null)
                _entityManager = GetComponent<EntityManager>();
            return _entityManager;
        }
    }

    private CharacterController character_controller;
    private CharacterController characterController
    {
        get
        {
            if (character_controller == null && entityManager != null)
            {
                character_controller = entityManager.characterController;
            }
            return character_controller;
        }
    }

    private Animator _animator;
    private Animator animator
    {
        get
        {
            if (_animator == null && entityManager != null)
                _animator = entityManager.animator;
            return _animator;
        }
    }

    private Transform child_0;
    private Transform child
    {
        get
        {
            if (child_0 == null)
                child_0 = transform.GetChild(0);
            return child_0;
        }
    }
    private void Awake()
    {
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.LeftShift, "UnSit", UnSit, ActionKeyType.Down);
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.RightShift, "UnSit", UnSit, ActionKeyType.Down);
    }
    private void OnDestroy()
    {
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.LeftShift, "UnSit", UnSit, ActionKeyType.Down);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.RightShift, "UnSit", UnSit, ActionKeyType.Down);
    }
    public void Sit(Vector3 target_position)
    {
        characterController.enabled = false;
        transform.position = target_position;
        PlaySitAnim(true);
        entityManager.networkAnimationStatus = NetworkAnimationValue.SIT;
    }
    public void SitRotation(Vector3 target_rotation)
    {
        transform.GetChild(0).rotation = Quaternion.Euler(target_rotation);
    }
    public Action OnUnsitCallback;
    public void UnSit()
    {
        PlaySitAnim(false);
        entityManager.networkAnimationStatus = NetworkAnimationValue.IDLE;
        characterController.enabled = true;
        OnUnsitCallback.Invoke();
    }
    public void PlaySitAnim(bool is_sit)
    {
        animator.SetBool("isSit", is_sit);
    }
}
