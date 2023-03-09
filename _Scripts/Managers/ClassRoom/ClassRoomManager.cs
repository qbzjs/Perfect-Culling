using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using Colyseus;
using Colyseus.Schema;
using NativeWebSocket;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class ClassRoomManager : MonoBehaviour
{

    [SerializeField]
    private Transform playerParent;
    [SerializeField]
    private Transform otherPlayerParent;
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private ClassRoomBoardComponent board;
    [SerializeField]
    private ClassRoomRemoteComponent remote;

    private ColyseusRoom<ClassRoomState> classRoom;
    private Dictionary<ObscuredString, NetworkEntity> lstPlayers = new Dictionary<ObscuredString, NetworkEntity>();
    private ObscuredString _ourEntityId;
    private ClassRoomRole _ourEntityRole = ClassRoomRole.none;
    private ClassRoomMediaType currentMediaType = ClassRoomMediaType.video;

    private void Awake()
    {
        UserDatas.current_room_type = CurrentRoomType.Class;
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.LeftAlt, "ActiveCursor", ActiveCursor, ActionKeyType.Down);
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.LeftAlt, "DeactiveCursor", DeactiveCursor, ActionKeyType.Up);
        Observer.Instance.AddObserver(ObserverKey.ClassRoomResetVideo, ResetVideo);
    }

    private void OnDestroy()
    {
        ClearPlayers();
        UnregisterClassRoomEvents();
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.LeftAlt, "ActiveCursor", ActiveCursor, ActionKeyType.Down);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.LeftAlt, "DeactiveCursor", DeactiveCursor, ActionKeyType.Up);
        Observer.Instance.RemoveObserver(ObserverKey.ClassRoomResetVideo, ResetVideo);
        if (classRoom != null)
            classRoom.Leave();
    }

    // Start is called before the first frame update
    void Start()
    {
        TPRLSoundManager.Instance.StopMusic();
        StartCoroutine(IEInit(delegate
        {
            DeactiveCursor();
            RegisterClassRoomHandlers();
        }));
    }

    private IEnumerator IEInit(UnityAction action)
    {
        yield return new WaitUntil(() => ClassRoomCanvasManager.instance != null);
        if (mainCamera != null)
        {
            var cam = mainCamera.GetUniversalAdditionalCameraData();
            cam.cameraStack.Add(ClassRoomCanvasManager.instance.canvasCamera);
        }
        yield return new WaitUntil(() => NetworkingManager.Instance.classRoom != null);
        action?.Invoke();
    }

    private void RegisterClassRoomHandlers()
    {
        classRoom = NetworkingManager.Instance.classRoom;
        if (classRoom != null)
        {
            UpdateCurrentMode(classRoom.State.project_current_mode);
            StartCoroutine(IESpawnAllPlayer());
            StartCoroutine(IEInitResources(classRoom.State.projector_modes));
            classRoom.OnLeave += OnLeaveRoom;
            classRoom.OnStateChange += OnClassRoomStateChange;
            classRoom.State.users.OnAdd += NetworkedUsers_OnAdd;
            classRoom.State.users.OnRemove += NetworkedUsers_OnRemove;
        }
        else
        {
            Debug.LogError($"Cannot register class room handlers, room is null!");
        }
    }

    private void UnregisterClassRoomEvents()
    {
        if (classRoom != null)
        {
            classRoom.OnLeave -= OnLeaveRoom;
            classRoom.OnStateChange -= OnClassRoomStateChange;
            classRoom.State.users.OnAdd -= NetworkedUsers_OnAdd;
            classRoom.State.users.OnRemove -= NetworkedUsers_OnRemove;
        }
        else
        {
            Debug.LogError($"Cannot unregister class room handlers, room is null!");
        }
    }

    private void OnLeaveRoom(int code)
    {
        WebSocketCloseCode closeCode = WebSocketHelpers.ParseCloseCodeEnum(code);
    }
    private void OnClassRoomStateChange(ClassRoomState state, bool isfirststate)
    {
        UpdateCurrentMode(classRoom.State.project_current_mode);
        if (board != null)
        {
            board.SwitchMedia(currentMediaType);
            board.UpdateEvents(state.projector_modes);
        }
        if (remote != null)
            remote.UpdateEvents(currentMediaType, state.projector_modes);
    }
    private void NetworkedUsers_OnAdd(string key, ClassEntityState value)
    {
        StartCoroutine(CreatePlayer(value.id, value.ow_ss_id, string.IsNullOrEmpty(value.role) ? ClassRoomRole.none : Ultis.ParseEnum<ClassRoomRole>(value.role)));
    }

    private IEnumerator IESpawnAllPlayer()
    {
        yield return new WaitUntil(() => classRoom.State.users.Count >= 0);
        classRoom.State.users.ForEach((key, value) =>
        {
            StartCoroutine(CreatePlayer(value.id, value.ow_ss_id, string.IsNullOrEmpty(value.role) ? ClassRoomRole.none : Ultis.ParseEnum<ClassRoomRole>(value.role)));
        });
    }
    private IEnumerator CreatePlayer(ObscuredString class_session_id, ObscuredString ow_session_id, ClassRoomRole role)
    {
        yield return new WaitUntil(() => classRoom.State.users.ContainsKey(class_session_id));
        ObscuredBool isOurs = class_session_id.Equals(classRoom.SessionId);
        NetworkedEntityState owEntityState = NetworkingManager.Instance.openworldRoom.State.users[ow_session_id];
        ClassEntityState classEntityState = classRoom.State.users[class_session_id];

        if (!lstPlayers.ContainsKey(class_session_id))
        {
            if (UpdateOurGameEntity(owEntityState, isOurs, role) == false)
                SpawnEntity(owEntityState, isOurs, role);
        }
    }

    private void SpawnEntity(NetworkedEntityState entityState, ObscuredBool is_me, ClassRoomRole role)
    {
        Vector3 position = new Vector3(17.8f, 1, -11.752f);
        RecordUnitInfo info = DataController.Instance.Player_VO.GetDataByName<RecordUnitInfo>("PlayerInfo");
        GameObject ob = PrefabsManager.Instance.GetAsset<GameObject>($"Player_{UserDatas.user_Data.info.current_selected_character}");
        EntityManager player;
        if (is_me)
        {
            player = CreateController.instance.CreateObjectGetComponent<EntityManager>(ob, Vector3.zero, playerParent, "MePlayer");
        }
        else
        {
            player = CreateController.instance.CreateObjectGetComponent<EntityManager>(ob, Vector3.zero, otherPlayerParent, "OtherPlayer");
        }

        if (player != null)
        {
            NetworkEntity entity = player.GetComponent<NetworkEntity>();
            entity.Init(entityState, is_me, false);
            lstPlayers.Add(entityState.id, entity);
            if (is_me)
            {
                _ourEntityId = entityState.id;
                _ourEntityRole = role;
                if (board != null)
                    board.SetTeacher(role.ToString().ToLower().Equals("teacher"));
                Observer.Instance.Notify(ObserverKey.ClassRoomUpdateUI, role);
            }
            CharacterController characterController = player.GetComponent<CharacterController>();
            characterController.enabled = false;
            player.transform.localPosition = position;
            characterController.enabled = true;
            player.info = info;
            player.classRole = role;
            
        }
    }

    private ObscuredBool UpdateOurGameEntity(NetworkedEntityState state, bool is_me, ClassRoomRole role)
    {
        ObscuredString id = state.id;
        if (lstPlayers.ContainsKey(id))
        {
            NetworkEntity entity = lstPlayers[id];
            lstPlayers.Remove(_ourEntityId);

            entity.Init(state, is_me, false);
            EntityManager player = entity.GetComponent<EntityManager>();
            if (player != null)
                player.classRole = role;
            if (is_me)
            {
                _ourEntityId = state.id;
                _ourEntityRole = role;
                if (board != null)
                    board.SetTeacher(role.ToString().ToLower().Equals("teacher"));
                Observer.Instance.Notify(ObserverKey.ClassRoomUpdateUI, role);
            }

            lstPlayers.Add(id, entity);

            return true;
        }
        return false;
    }


    private void NetworkedUsers_OnRemove(string key, ClassEntityState value)
    {
        RemoveGameEntity(value.ow_ss_id);
    }

    private void RemoveGameEntity(ObscuredString ow_id)
    {
        if (lstPlayers.ContainsKey(ow_id))
        {
            NetworkEntity entity = lstPlayers[ow_id];
            lstPlayers.Remove(ow_id);
            entity.UnregisterStateEvent();
            Destroy(entity.gameObject);
        }
    }

    private void ClearPlayers()
    {
        if (lstPlayers == null || lstPlayers.Count == 0) return;
        foreach (var item in lstPlayers)
        {
            NetworkEntity networkEntity = item.Value;
            if (networkEntity == null) continue;
            networkEntity.UnregisterStateEvent();
            Destroy(networkEntity.gameObject);
        }
        lstPlayers.Clear();
    }

    private void UpdateCurrentMode(string mode)
    {
        string current_media_mode = mode;
        if (!string.IsNullOrEmpty(current_media_mode))
        {
            if (current_media_mode.ToLower().Equals(ClassRoomMediaType.video.ToString()))
                currentMediaType = ClassRoomMediaType.video;
            else if (current_media_mode.ToLower().Equals(ClassRoomMediaType.slide.ToString()))
                currentMediaType = ClassRoomMediaType.slide;
        }
    }

    private IEnumerator IEInitResources(MapSchema<ProjectorModeState> states)
    {
        yield return new WaitUntil(() => states.Count > 0);
        if (board != null)
        {
            board.ResetDefault();
            board.SwitchMedia(currentMediaType);
            board.onVideoStop = OnVideoStop;
        }
        if (remote != null)
            remote.UpdateEvents(currentMediaType, states);

        states.ForEach((key, value) =>
        {
            if (value.mode.ToLower().Equals(ClassRoomMediaType.slide.ToString()))
            {
                if (board != null)
                {
                    board.PrepareSlide(value);
                    board.PrepareQuestion(value);
                }
            }
            else if (value.mode.ToLower().Equals(ClassRoomMediaType.video.ToString()))
            {
                if (board != null)
                    board.PrepareVideo(value);
            }
        });
    }

    private void OnVideoStop()
    {
        if (remote != null)
            remote.InteractRemote(ClassRoomRemoteInteactionType.Pause);
    }

    private void ResetVideo(object data)
    {
        if (board != null)
            board.ResetVideo();
    }

    private void ActiveCursor()
    {
        Ultis.SetActiveCursor(true);
    }
    private void DeactiveCursor()
    {
        Ultis.SetActiveCursor(false);
    }


}
