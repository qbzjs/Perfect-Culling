using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Time;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementComponent : MonoBehaviour
{
    private ObscuredFloat move_speed = 0;

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

    private bool isRunning = false;
    public bool isSprint = false;

    private void RegisterEvent()
    {
        if (_isPlayer)
        {
            InputRegisterEvent.Instance.RegisterEvent(KeyCode.LeftShift, "Sprint", Sprint, ActionKeyType.Down);
            InputRegisterEvent.Instance.RegisterEvent(KeyCode.LeftShift, "SetDefaultMoveSpeed", SetDefaultMoveSpeed, ActionKeyType.Up);
        }
    }
    private ObscuredBool _isPlayer;
    public void SetUpBehaviour(ObscuredBool isPlayer)
    {
        this._isPlayer = isPlayer;
        RegisterEvent();
    }
    public void UpdateInfo()
    {
        if (isSprint)
            Sprint();
        else
            SetDefaultMoveSpeed();
    }

    private Vector3 _moveDirection;
    private Vector3 _moveDirectionRef;
    public void OnMove(InputValue value)
    {
        var input = value.Get<Vector2>();
        _moveDirection = new Vector3(input.x, 0, input.y);
        if (_moveDirection.magnitude > 0)
        {
            _moveDirectionRef = _moveDirection;
        }
        if (input.magnitude > 0)
        {
            if (isSprint) move_speed = entityManager.isDriving ? entityManager.info.drive_sprint_speed : entityManager.info.sprint_speed;
            else move_speed = entityManager.isDriving ? entityManager.info.drive_move_speed : entityManager.info.move_speed;
        }
        else move_speed = 0;
        SetAnim();
    }
    private Vector3 currentInputVector;
    private Vector3 smoothInputVelocity;
    private float smoothInputSpeed = .2f;
    private float smoothVelocitySpeed = 1f;
    private float currentSpeed = .2f;
    private float currentSpeedVelocityref;

    private Vector3 CameraDirection(Vector3 movementDirection)
    {
        var mainCamera = Camera.main;
        var cameraForward = mainCamera.transform.forward;
        var cameraRight = mainCamera.transform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        return cameraForward * movementDirection.z + cameraRight * movementDirection.x;
    }
    private Vector3 PlayerDirection(Vector3 movementDirection)
    {
        var cameraForward = child.transform.forward;

        cameraForward.y = 0f;

        return cameraForward * movementDirection.z;
    }


    private void Update()
    {
        if (!_isPlayer) return;
        Move();
        RotatePlayer();
    }
    private void Move()
    {
        Vector3 move;
        if (entityManager.isDriving)
        {
            move = PlayerDirection(_moveDirectionRef).normalized;
        }
        else
        {
            move = CameraDirection(_moveDirection).normalized;
        }
        if (currentSpeed >= 0.1f && !isRunning)
        {
            isRunning = true;
            Observer.Instance.Notify(ObserverKey.MoveCharacter, isSprint);
        }
        else if (isRunning && currentSpeed <= 0.1f)
        {
            isRunning = false;
            Observer.Instance.Notify(ObserverKey.MoveCharacter, null);
        }
        currentSpeed = Mathf.SmoothDamp(currentSpeed, move_speed, ref currentSpeedVelocityref, smoothVelocitySpeed);
        if (characterController != null && characterController.enabled == true)
            characterController.Move(move * currentSpeed * SpeedHackProofTime.deltaTime);

    }
    private void RotatePlayer()
    {
        Vector3 move = CameraDirection(_moveDirection).normalized;

        if (_moveDirection != Vector3.zero)
        {
            if (!entityManager.isDriving)
            {
                smoothInputSpeed = 0.2f;
                currentInputVector = Vector3.SmoothDamp(currentInputVector, move, ref smoothInputVelocity, smoothInputSpeed);
                child.LookAt(child.position + currentInputVector);
            }
            else
            {
                if (_moveDirection.z != -1f && _moveDirection.z != 1)
                {
                    smoothInputSpeed = 0.5f;
                    //currentInputVector = Vector3.SmoothDamp(currentInputVector, move, ref smoothInputVelocity, smoothInputSpeed);
                    var targetRotation = Quaternion.LookRotation(move);
                    child.rotation = Quaternion.LerpUnclamped(child.rotation, targetRotation, 0.005f);
                }
            }
        }
    }

    public void SetMoveAnim(bool is_move)
    {
        if (animator == null) return;
        if (entityManager.isDriving)
        {
            if (animator.GetCurrentAnimatorClipInfo(0).Length > 0 && !(animator.GetCurrentAnimatorClipInfo(0)[0].clip.name is "Sit"))
            {
                animator.SetBool("isMoving", false);
                animator.SetBool("isSprint", false);
                animator.SetBool("isSit", true);
            }
            return;
        }
        if (isSprint)
        {
            if (!(animator.GetCurrentAnimatorClipInfo(0)[0].clip.name is ("Walking" or "Running" or "Jump" or "Idle")))
            {
                animator.Play("Running");
            }
            animator.SetBool("isMoving", is_move);
            animator.SetBool("isSprint", is_move);
        }
        else
        {
            if (!(animator.GetCurrentAnimatorClipInfo(0)[0].clip.name is ("Walking" or "Running" or "Jump" or "Idle")))
            {
                animator.Play("Walking");
            }
            animator.SetBool("isMoving", is_move);
            animator.SetBool("isSprint", false);
        }
        //if (isMoving)
        //{
        //    TPRLSoundManager.Instance.PlaySoundFx(SoundName.MOVESTEP, false);
        //}
    }

    private void Sprint()
    {
        isSprint = true;
        move_speed = entityManager.isDriving ? entityManager.info.drive_sprint_speed : entityManager.info.sprint_speed;
        currentSpeed = entityManager.isDriving ? currentSpeed : move_speed;
        Observer.Instance.Notify(ObserverKey.MoveCharacter, isRunning ? isSprint : null);
        SetAnim();
    }

    private void SetDefaultMoveSpeed()
    {
        isSprint = false;
        move_speed = entityManager.isDriving ? entityManager.info.drive_move_speed : entityManager.info.move_speed;
        currentSpeed = entityManager.isDriving ? currentSpeed : move_speed;
        Observer.Instance.Notify(ObserverKey.MoveCharacter, isRunning ? isSprint : null);
        SetAnim();
    }
    private void SetAnim()
    {
        float magnitude = _moveDirection.magnitude;
        bool is_move = magnitude > 0;
        SetMoveAnim(is_move);
        if (entityManager.networkAnimationStatus == NetworkAnimationValue.JUMP) return;
        if (is_move)
        {
            if (isSprint)
            {
                entityManager.networkAnimationStatus = NetworkAnimationValue.RUN;
            }
            else
            {
                entityManager.networkAnimationStatus = NetworkAnimationValue.WALK;
            }
        }
        else
        {
            entityManager.networkAnimationStatus = NetworkAnimationValue.IDLE;
        }
        if (entityManager.isDriving)
        {
            if (!(animator.GetCurrentAnimatorClipInfo(0)[0].clip.name is "Sit"))
            {
                entityManager.networkAnimationStatus = NetworkAnimationValue.SIT;
            }
            return;
        }
    }
    private void OnDestroy()
    {
        if (_isPlayer)
        {
            InputRegisterEvent.Instance.RemoveEventKey(KeyCode.LeftShift, "Sprint", Sprint, ActionKeyType.Down);
            InputRegisterEvent.Instance.RemoveEventKey(KeyCode.LeftShift, "SetDefaultMoveSpeed", SetDefaultMoveSpeed, ActionKeyType.Up);
        }
    }
}
