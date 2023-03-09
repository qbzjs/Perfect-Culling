using System;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using Colyseus;
using NativeWebSocket;
using UnityEngine;

using UnityEngine.Events;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Cinemachine.CinemachineBrain cinmachine_Brain;
    [SerializeField] private Transform playerParent;
    [SerializeField]
    private Transform otherPlayerParent;
    private ObscuredString _ourEntityId;
    private UnityAction OnRegisterEventOpenworldDone = null;

    private VehiclesManager _vehiclesManager;
    private VehiclesManager vehiclesManager
    {
        get
        {
            if (_vehiclesManager == null)
                _vehiclesManager = GetComponentInChildren<VehiclesManager>();
            return _vehiclesManager;
        }
    }
    private EntityManager _playerMe;
    public EntityManager playerMe => _playerMe;
    private CharacterRig _characterRig;
    public CharacterRig characterRig => _characterRig;

    private Animator _animatorMe;
    private Animator animatorMe
    {
        get
        {
            if (_animatorMe == null && _playerMe != null)
                _animatorMe = _playerMe.animator;
            return _animatorMe;
        }
    }

    private Dictionary<ObscuredString, NetworkEntity> lstPlayers = new Dictionary<ObscuredString, NetworkEntity>();

    private ColyseusRoom<OWRoomState> openworldRoom;
    private bool _checkStatusBtWheel = false;


    private void Awake()
    {
        Observer.Instance.AddObserver(ObserverKey.ChangeCharacter, ChangeCharacter);
        Observer.Instance.AddObserver(ObserverKey.SetStatusEquipVehicle, SetStatusButtonWheel);
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.B, "SetStatusButtonWheel", SetStatusButtonWheel, ActionKeyType.Up);
    }

    private void OnDestroy()
    {
        Observer.Instance.RemoveObserver(ObserverKey.ChangeCharacter, ChangeCharacter);
        Observer.Instance.RemoveObserver(ObserverKey.SetStatusEquipVehicle, SetStatusButtonWheel);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.B, "SetStatusButtonWheel", SetStatusButtonWheel, ActionKeyType.Up);
        StopAllCoroutines();
        ClearPlayers();
        UnregisterOpenworldRoomEvents();
    }

    public void RegisterOpenworldRoomEvents(UnityAction on_done)
    {
        OnRegisterEventOpenworldDone = on_done;
        openworldRoom = NetworkingManager.Instance.openworldRoom;
        if (openworldRoom != null)
        {
            openworldRoom.OnLeave += OnLeaveRoom;

            openworldRoom.OnStateChange += OnGameRoomStateChange;
            openworldRoom.State.users.OnAdd += NetworkedUsers_OnAdd;
            openworldRoom.State.users.OnRemove += NetworkedUsers_OnRemove;
            StartCoroutine(IESpawnAllPlayer());
        }
        else
        {
            Debug.LogError($"Cannot register room handlers, room is null!");
        }
    }

    private void UnregisterOpenworldRoomEvents()
    {
        if (openworldRoom != null)
        {
            openworldRoom.OnLeave -= OnLeaveRoom;
            openworldRoom.OnStateChange -= OnGameRoomStateChange;
            openworldRoom.State.users.OnAdd -= NetworkedUsers_OnAdd;
            openworldRoom.State.users.OnRemove -= NetworkedUsers_OnRemove;
        }
        else
        {
            Debug.LogError($"Cannot unregister room handlers, room is null!");
        }
    }

    private void OnLeaveRoom(int code)
    {
        WebSocketCloseCode closeCode = WebSocketHelpers.ParseCloseCodeEnum(code);
    }

    private IEnumerator IESpawnAllPlayer()
    {
        yield return new WaitUntil(() => openworldRoom.State.users.Count >= 0);
        openworldRoom.State.users.ForEach((key, value) =>
        {
            if (value.visible_ow)
                StartCoroutine(CreatePlayer(value.id));
        });
    }
    private IEnumerator CreatePlayer(ObscuredString player_session_id)
    {
        yield return new WaitUntil(() => openworldRoom.State.users.ContainsKey(player_session_id));
        ObscuredBool isOurs = player_session_id.Equals(openworldRoom.SessionId);
        NetworkedEntityState entityState = openworldRoom.State.users[player_session_id];

        if (!lstPlayers.ContainsKey(player_session_id))
        {
            if (UpdateOurGameEntity(entityState, isOurs) == false)
                SpawnEntity(entityState, isOurs);
        }
    }
    private void SpawnEntity(NetworkedEntityState entityState, ObscuredBool is_me)
    {
        Vector3 position = new Vector3(26.08505f, 2, -106.9492f);
        ObscuredVector3 stateposition = new ObscuredVector3(entityState.xPos, entityState.yPos, entityState.zPos);
        RecordUnitInfo info = DataController.Instance.Player_VO.GetDataByName<RecordUnitInfo>("PlayerInfo");
        GameObject ob = PrefabsManager.Instance.GetAsset<GameObject>("Player_10");
        GameObject hips = PrefabsManager.Instance.GetAsset<GameObject>("Hips");
        if (is_me)
        {
            CreatePlayerWithRig(ob, hips, playerParent, "MePlayer");
        }
        else
        {
            CreatePlayerWithRig(ob, hips, otherPlayerParent, "OtherPlayer");
        }
        if (playerMe != null)
        {
            NetworkEntity entity = playerMe.GetComponent<NetworkEntity>();
            entity.Init(entityState, is_me, true);
            lstPlayers.Add(entityState.id, entity);
            CharacterController characterController = playerMe.GetComponent<CharacterController>();
            characterController.enabled = false;
            playerMe.name = entityState.id;
            playerMe.transform.localPosition = is_me ? position : stateposition;
            if (is_me)
            {
                _ourEntityId = entityState.id;
                characterController.enabled = true;
                OnRegisterEventOpenworldDone?.Invoke();
            }
            playerMe.info = info;
        }

    }
    private void CreatePlayerWithRig(GameObject ob, GameObject hips, Transform parent, string tag)
    {
        _playerMe = CreateController.instance.CreateObjectGetComponent<EntityManager>(ob, Vector3.zero, parent, tag);
        _characterRig = CreateController.instance.CreateObjectGetComponent<CharacterRig>(hips, Vector3.zero, _playerMe.transform.GetChild(0), tag);
        _playerMe.characterRig = _characterRig;
        int length = Enum.GetValues(typeof(InventoryItemType)).Length;
        for (int i = 0; i < length; i++)
        {
            InventoryItemType type = (InventoryItemType)i;
            RecordItemEquip record = UserDatas.GetRecordItemEquipTrueByType(type);
            if(record.itemType== type)
            _playerMe.equipment.CheckEquipItem(record);
        }
    }
    private void RemoveGameEntity(ObscuredString id)
    {
        if (lstPlayers.ContainsKey(id))
        {
            NetworkEntity entity = lstPlayers[id];
            lstPlayers.Remove(id);
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

    private ObscuredBool UpdateOurGameEntity(NetworkedEntityState state, bool is_me)
    {
        ObscuredString id = state.id;
        if (lstPlayers.ContainsKey(id))
        {
            NetworkEntity entity = lstPlayers[id];
            lstPlayers.Remove(_ourEntityId);
            entity.name = id;
            entity.Init(state, is_me, true);
            if (is_me)
                _ourEntityId = state.id;

            lstPlayers.Add(id, entity);

            return true;
        }
        return false;
    }

    private void OnGameRoomStateChange(OWRoomState state, bool isfirststate)
    {
        if (state == null || state.users == null) return;
        state.users.ForEach((key, value) =>
        {
            if (lstPlayers.ContainsKey(value.id))
            {
                if (value.visible_ow == false)
                {
                    RemoveGameEntity(value.id);
                }
            }
            else
            {
                if (value.visible_ow)
                {
                    CreatePlayer(value.id);
                }
            }
        });
    }

    private void NetworkedUsers_OnAdd(string key, NetworkedEntityState value)
    {
        StartCoroutine(CreatePlayer(value.id));
    }
    private void NetworkedUsers_OnRemove(string key, NetworkedEntityState value)
    {
        RemoveGameEntity(value.id);
    }

    private void ChangeCharacter(object data)
    {
        return;
        if (data == null) return;
        RecordNetworkCharacter recordNetworkCharacter = (RecordNetworkCharacter)data;
        foreach (KeyValuePair<ObscuredString, NetworkEntity> pair in lstPlayers)
        {
            ObscuredString id = pair.Key;
            NetworkedEntityState entityState = openworldRoom.State.users[id];
            RemoveGameEntity(id);
            SpawnEntity(entityState, recordNetworkCharacter.isOurs);
        }
    }
    private void SetStatusButtonWheel(object data)
    {
        if (data != null)
        {
            RecordItemEquip record = (RecordItemEquip)data;
            _checkStatusBtWheel = record.is_equip;
            CheckEquipVehicle(record);
            UserDatas.SetEquipItem(record);
        }
    }


    private void SetStatusButtonWheel()
    {
        _checkStatusBtWheel = !_checkStatusBtWheel;
        RecordItemEquip record = UserDatas.current_vehicle;
        record.is_equip = _checkStatusBtWheel;
        CheckEquipVehicle(record);
        UserDatas.SetEquipItem(record);
    }

    private void CheckEquipVehicle(RecordItemEquip record)
    {
        if (_checkStatusBtWheel)
        {
            if (vehiclesManager != null)
                vehiclesManager.EquipVehicle(record, playerMe.transform.position, playerMe.Model().transform.rotation, (record_item_) =>
                {
                    cinmachine_Brain.m_DefaultBlend.m_Time = 0.5f;
                    animatorMe.SetBool("isSit", true);
                    playerMe.BlockPlayerInput(true);
                    playerMe.transform.SetParent(vehiclesManager.currentVehicle.transform);
                    bool is_hide_player = record_item_.is_hide_player;
                    playerMe.SetActiveTextName(!is_hide_player);
                    playerMe.characterController.center = new Vector3(0, 10000, 0);
                    //model_position = new Vector3(0, 0.28f, 0);
                    playerMe.SetDriving(true);
                    playerMe.SetActiveModelMesh(!is_hide_player);
                });
        }
        else
        {
            cinmachine_Brain.m_DefaultBlend.m_Time = 1f;
            playerMe.characterController.center = new Vector3(0, -0.29f, 0);
            playerMe.transform.SetParent(playerParent);
            if (vehiclesManager != null)
                vehiclesManager.UnEquipVehicle();
            animatorMe.SetBool("isSit", false);
            playerMe.SetActiveTextName(true);
            playerMe.SetDriving(false);
            playerMe.SetActiveModelMesh(true);
            playerMe.BlockPlayerInput(false);
        }
    }
}
