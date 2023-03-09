using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Time;

public class JumpComponent : MonoBehaviour
{
    private ObscuredFloat jumpPower = 0;
    public ObscuredBool isGrounded = true;
    private Vector3 _velocity;
    private ObscuredFloat gravity = -9.81f;
    private ObscuredFloat GroundedOffset = -0.14f;
    private ObscuredFloat GroundedRadius = 0.28f;
    [SerializeField]
    private LayerMask GroundLayers;
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

    public void UpdateInfo()
    {
        jumpPower = entityManager.info.jump_info.jump_power;
        gravity = entityManager.info.jump_info.gravity;
        GroundedOffset = entityManager.info.jump_info.grounded_offset;
        GroundedRadius = entityManager.info.jump_info.grounded_radius;
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            if (entityManager.isDriving) return;
            Jump();
        }
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        ObscuredVector3 spherePosition = new ObscuredVector3(transform.position.x, transform.position.y - GroundedOffset,
            transform.position.z);
        isGrounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);
    }
    private bool isJumping = false;
    private void FixedUpdate()
    {
        GroundedCheck();
        if (isGrounded && _velocity.y < 0)
        {
            _velocity.y = 0f;
            SetJumpAnim(false);
            if (isJumping) {
                isJumping = false;
                entityManager.networkAnimationStatus = NetworkAnimationValue.IDLE;
            }
        }
        ApplyGravity();
    }

    private void Jump()
    {
        if (isGrounded == false) return;
        _velocity.y = Mathf.Sqrt(jumpPower * -2f * gravity) - 0.5f;
        SetJumpAnim(true);
        isJumping = true;
        entityManager.networkAnimationStatus = NetworkAnimationValue.JUMP;
    }

    private void ApplyGravity()
    {
        _velocity.y += (gravity) * SpeedHackProofTime.deltaTime;
        if (characterController != null && characterController.enabled == true)
            characterController.Move(_velocity * SpeedHackProofTime.deltaTime);

    }
    public void SetJumpAnim(bool isJumping)
    {
        if (animator != null)
            animator.SetBool("isJumping", isJumping);
    }
}
