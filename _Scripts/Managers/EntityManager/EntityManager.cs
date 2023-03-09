using CodeStage.AntiCheat.ObscuredTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
public class EntityManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro txName;
    protected RecordUnitInfo _info;

    public ObscuredBool isMe;
    [SerializeField]
    private ObscuredBool isNPC;
    [SerializeField] private GameObject ob_Cam;
    [SerializeField] GameObject ob_ModelMesh;
    [SerializeField] GameObject ob_Model;
    public Transform parentHip;
    public CharacterRig characterRig { get; set; }
    public NetworkAnimationValue networkAnimationStatus;
    public RecordUnitInfo info
    {
        set
        {
            _info = value;
            UpdateInfo();
        }
        get
        {
            return _info;
        }
    }

    protected virtual void UpdateInfo()
    {
        if (movement != null)
            movement.UpdateInfo();
        if (jump != null)
            jump.UpdateInfo();
        if (interact != null)
            interact.UpdateInfo();
        //SetName();
    }

    private MovementComponent _movement;
    private MovementComponent movement
    {
        get
        {
            try
            {
                if (_movement == null)
                    _movement = GetComponent<MovementComponent>();
                return _movement;
            }
            catch (Exception e) { return null; }
        }
    }
    private SitComponent _sit;
    public SitComponent sit
    {
        get
        {
            try
            {
                if (_sit == null)
                    _sit = GetComponent<SitComponent>();
                return _sit;
            }
            catch (Exception e) { return null; }
        }
        set
        {
            _sit = value;
        }
    }

    private JumpComponent _jump;
    private JumpComponent jump
    {
        get
        {
            try
            {
                if (_jump == null)
                    _jump = GetComponent<JumpComponent>();
                return _jump;
            }
            catch (Exception e) { return null; }
        }
    }

    private InteractComponent _interact;
    private InteractComponent interact
    {
        get
        {
            try
            {
                if (_interact == null)
                    _interact = GetComponent<InteractComponent>();
                return _interact;
            }
            catch (Exception e) { return null; }
        }
    }
    private EquipmentComponent _equipment;
    public EquipmentComponent equipment
    {
        get
        {
            try
            {
                if (_equipment == null)
                    _equipment = GetComponent<EquipmentComponent>();
                return _equipment;
            }
            catch (Exception e) { return null; }
        }
    }
    private EmotionComponent _emotion;
    private EmotionComponent emotion
    {
        get
        {
            try
            {
                if (_emotion == null)
                    _emotion = GetComponent<EmotionComponent>();
                return _emotion;
            }
            catch (Exception e) { return null; }
        }
    }

    private VoiceChatComponent _voiceChatComponent;
    private VoiceChatComponent voiceChatComponent
    {
        get
        {
            if (_voiceChatComponent == null) _voiceChatComponent = GetComponent<VoiceChatComponent>();
            return _voiceChatComponent;
        }
    }
    private CameraRotationComponent _cameraRotation;
    private CameraRotationComponent cameraRotation
    {
        get
        {
            try
            {
                if (_cameraRotation == null)
                    _cameraRotation = GetComponent<CameraRotationComponent>();
                return _cameraRotation;
            }
            catch (Exception e) { return null; }
        }
    }

    private CharacterController character_controller;
    public CharacterController characterController
    {
        get
        {
            if (character_controller == null)
            {
                character_controller = GetComponent<CharacterController>();
            }
            return character_controller;
        }
    }
    private PlayerInput _playerInput;
    private PlayerInput playerInput
    {
        get
        {
            if (_playerInput == null)
            {
                _playerInput = GetComponent<PlayerInput>();
            }
            return _playerInput;
        }
    }

    private Animator _animator;
    public Animator animator
    {
        get
        {
            if (_animator == null)
                _animator = GetComponentInChildren<Animator>(true);
            return _animator;
        }
    }

    private AudioListener _audioListener;
    public AudioListener audioListener
    {
        get
        {
            if (_audioListener == null)
                _audioListener = GetComponent<AudioListener>();
            return _audioListener;
        }
    }


    private bool _isDriving = false;
    public bool isDriving => _isDriving;


    private ClassRoomRole _classRole = ClassRoomRole.none;
    public ClassRoomRole classRole
    {
        get => _classRole;
        set => _classRole = value;
    }

    private string _id;
    public string ID
    {
        get
        {
            return _id;
        }
    }
    public void SetUp(ObscuredBool is_me, ObscuredString _id, ObscuredBool is_visible_ow)
    {
        this.isMe = is_me;
        this._id = _id;
        if (interact != null) interact.SetUpBehaviour(is_me);
        if (equipment != null) equipment.SetUpBehaviour(is_me);
        if (emotion != null) emotion.SetUpBehaviour(is_me);
        if (movement != null) movement.SetUpBehaviour(is_me);
        if (voiceChatComponent != null) voiceChatComponent.SetUpBehaviour(is_me, is_visible_ow);
        characterController.enabled = (ObscuredBool)true;
        playerInput.enabled = is_me;
        if (is_me)
        {
            RegisterEvent();
        }
    }

    private void Awake()
    {
        if (isNPC)
        {
            RegisterEvent();
        }
    }

    private void RegisterEvent()
    {
        Observer.Instance.AddObserver(ObserverKey.UpdateResponseInteractionInfo, UpdateResponseInteractionInfo);
        Observer.Instance.AddObserver(ObserverKey.GameBlockInput, BlockPlayerInput);
    }

    private void OnDestroy()
    {
        Observer.Instance.RemoveObserver(ObserverKey.UpdateResponseInteractionInfo, UpdateResponseInteractionInfo);
        Observer.Instance.RemoveObserver(ObserverKey.GameBlockInput, BlockPlayerInput);
    }


    private void Start()
    {

    }
    protected virtual void UpdateResponseInteractionInfo(object data)
    {
        if (data == null) return;
        RecordInteractionTypeObserver record_interact_type = (RecordInteractionTypeObserver)data;
        if (!record_interact_type.object_name.Equals(gameObject.name)) return;
        ResponseInteractionType response_type = record_interact_type.interaction_type;
        switch (response_type)
        {
            case ResponseInteractionType.Talk:
                Destroy(GetComponent<ResponseMissionInteractionComponent>());
                Destroy(GetComponent<ResponseQuizInteractionComponent>());
                //gameObject.AddComponent<ResponseTalkInteractionComponent>();
                break;
            case ResponseInteractionType.Quest:
                Destroy(GetComponent<ResponseTalkInteractionComponent>());
                Type type = Type.GetType(record_interact_type.mission_type);
                gameObject.AddComponent(type);
                break;
            case ResponseInteractionType.Quiz:
                Destroy(GetComponent<ResponseTalkInteractionComponent>());
                gameObject.AddComponent<ResponseQuizInteractionComponent>();
                break;
            default:
                Destroy(GetComponent<ResponseTalkInteractionComponent>());
                Destroy(GetComponent<ResponseSitMissionInteractionComponent>());
                Destroy(GetComponent<ResponseTalkMissionInteractionComponent>());
                Destroy(GetComponent<ResponseWatchVideoMissionInteractionComponent>());
                break;
        }
    }

    public void SetName(string text, string address)
    {
        if (txName != null)
        {
            name_player = text;
            if (string.IsNullOrEmpty(name_player))
            {
                txName.text = $"User#{address.Substring(address.Length - 4, 4)}";
            }
            string role = classRole == ClassRoomRole.none ? "" : classRole == ClassRoomRole.teacher ? "[<color=#000BFF>Teacher</color>]" : "[<color=#FFC200>Student</color>]";
            txName.text = $"{role}{name_player}";
        }
    }
    private string name_player;
    public string Name
    {
        get
        {
            return name_player;
        }
    }
    private void Update()
    {
        RotationTextName();
    }
    private void RotationTextName()
    {
        if (txName != null)
            txName.transform.rotation = Camera.main.transform.rotation;
    }

    public virtual void ClearEffects()
    {

    }

    public void BlockPlayerInput(object data)
    {
        if (data == null) return;
        bool enable = (bool)data;
        if (isDriving && !enable)
            return;
        if (playerInput != null)
        {
            playerInput.enabled = !enable;
        }
        if (characterController != null)
        {
            characterController.enabled = !enable;
        }
    }

    public void SetDriving(bool is_drive)
    {
        _isDriving = is_drive;
        if (movement != null)
            movement.UpdateInfo();
    }

    public void ResetAnim()
    {
        animator.SetBool("isMoving", false);
        animator.SetBool("isSprint", false);
        animator.SetBool("isSit", false);
        animator.SetBool("isJumping", false);
        animator.SetBool("isSit", false);
    }
    public void PlayIdle()
    {
        animator.Play("Idle");
    }
    public void SetSitAnim(bool is_sit)
    {
        if (sit != null)
        {
            sit.PlaySitAnim(is_sit);
        }
    }
    public void SetMoveAnim(bool is_move, bool isSprint)
    {
        movement.isSprint = isSprint;
        movement.SetMoveAnim(is_move);
    }
    public void SetJumpAnim(bool isJumping)
    {
        jump.SetJumpAnim(isJumping);
    }
    public void SetEmotion(int value)
    {
        emotion.PlayNetworkEmotion(value);
    }
    public void SetActiveTextName(bool active)
    {
        if (txName != null)
            txName.gameObject.SetActive(active);
    }
    public void SetActiveCamera(bool is_change)
    {
        if (ob_Cam != null)
            ob_Cam.SetActive(is_change);
    }

    public void SetEquipItem(object data)
    {
        if (equipment != null)
            equipment.CheckEquipItem(data);
    }

    public void SetActiveModelMesh(bool active)
    {
        GameObject mesh = gameObject.transform.GetChild(0).GetChild(1).gameObject;
        if (mesh != null)
            mesh.SetActive(active);

    }
    public GameObject Model()
    {
        if (ob_Model != null)
            return ob_Model;
        return null;
    }
}
