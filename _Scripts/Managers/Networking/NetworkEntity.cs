using Cinemachine;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Time;
using Colyseus.Schema;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NetworkedEntityState;

public class NetworkEntity : MonoBehaviour
{
    public ObscuredBool isMine;
    private NetworkedEntityState state;
    private NetworkedEntityState previousState;
    private NetworkedEntityState localUpdatedState;

    public CinemachineVirtualCamera virtualCamera;

    public ObscuredDouble interpolationBackTimeMs = 200f;
    public ObscuredDouble extrapolationLimitMs = 500f;
    public ObscuredFloat positionLerpSpeed = 10f;
    private ObscuredBool ignoreMovementSync = false;

    private EntityManager _entityManager;
    private EntityManager entityManager
    {
        get
        {
            if (_entityManager == null) _entityManager = GetComponent<EntityManager>();
            return _entityManager;
        }
    }

    [System.Serializable]
    private struct EntityState
    {
        public double timestamp;
        public Vector3 pos;
        public Quaternion rot;
    }

    // Clients store twenty states with "playback" information from the server. This
    // array contains the official state of this object at different times according to
    // the server.
    [SerializeField]
    private EntityState[] proxyStates = new EntityState[20];

    // Keep track of what slots are used
    private ObscuredInt proxyStateCount;

    private const float updateTimer = 0.05f;
    private ObscuredFloat currentUpdateTime = 0.0f;

    public ObscuredInt stateMove;
    private ObscuredInt lastStateMove;
    private ObscuredBool isVisibleOw = true;

    private void FixedUpdate()
    {
        if (isMine)
        {
            if (currentUpdateTime >= updateTimer)
            {
                currentUpdateTime = 0.0f;
                SyncServerWithView();
            }
            else
            {
                currentUpdateTime += SpeedHackProofTime.deltaTime;
            }
        }
        else
        {
            ProcessViewSync();
            if (currentUpdateTime >= updateTimer)
            {
                currentUpdateTime = 0.0f;
                SetAnimationValues();
                ChangeVehicle();
            }
            else
            {
                currentUpdateTime += SpeedHackProofTime.deltaTime;
            }
        }
    }
    private int stateVehicle = 255;
    private int lastVehicle = 255;
    RecordItemEquip currentVehicle;
    private void ChangeVehicle()
    {
        if (stateVehicle == lastVehicle) return;
        lastVehicle = stateVehicle;
        if (stateVehicle == 255)
        {
            currentVehicle.is_equip = false;
        }
        else
        {
            currentVehicle = new RecordItemEquip();
            currentVehicle.itemID = stateVehicle.ToString();
            currentVehicle.itemType = InventoryItemType.vehicle;
            currentVehicle.is_equip = true;
        }
        entityManager.SetEquipItem(currentVehicle);
    }
    private void SetAnimationValues()
    {
        if (stateMove != lastStateMove)
        {
            entityManager.ResetAnim();
        }
        lastStateMove = stateMove;
        switch (stateMove)
        {
            case (int)NetworkAnimationValue.IDLE:
                entityManager.SetSitAnim(false);
                entityManager.SetMoveAnim(false, false);
                entityManager.SetJumpAnim(false);
                entityManager.PlayIdle();
                break;
            case (int)NetworkAnimationValue.SIT:
                entityManager.SetSitAnim(true);
                break;
            case (int)NetworkAnimationValue.DRIVE:
                entityManager.SetSitAnim(true);
                break;
            case (int)NetworkAnimationValue.WALK:
                entityManager.SetMoveAnim(true, false);
                break;
            case (int)NetworkAnimationValue.RUN:
                entityManager.SetMoveAnim(true, true);
                break;
            case (int)NetworkAnimationValue.JUMP:
                entityManager.SetJumpAnim(true);
                break;
            default:
                entityManager.SetEmotion(stateMove);
                break;
        }
    }

    private void OnDestroy()
    {
        UnregisterStateEvent();
    }

    public void UnregisterStateEvent()
    {
        if (state != null)
        {
            state.OnChange -= OnStateChange;
            state = null;
        }
    }

    public void Init(NetworkedEntityState state, ObscuredBool is_me, bool is_visible_ow)
    {
        UnregisterStateEvent();
        isMine = is_me;
        if (!is_me)
        {
            Destroy(GetComponent<AudioListener>());
        }
        if (virtualCamera != null)
            virtualCamera.gameObject.SetActive(is_me);
        this.state = state;
        previousState = this.state;
        isVisibleOw = is_visible_ow;
        entityManager.SetUp(is_me,state.id, is_visible_ow);
        GameConfig.game_player = is_me;
        this.state.OnChange += OnStateChange;
    }
    private void SyncServerWithView()
    {
        previousState = state.Clone();

        //Copy Transform to State (round position to fix floating point issues with state compare)
        state.xPos = (float)System.Math.Round((decimal)transform.localPosition.x, 4);
        state.yPos = (float)System.Math.Round((decimal)transform.localPosition.y, 4);
        state.zPos = (float)System.Math.Round((decimal)transform.localPosition.z, 4);

        state.xRot = (float)System.Math.Round((decimal)transform.GetChild(0).rotation.x, 4);
        state.yRot = (float)System.Math.Round((decimal)transform.GetChild(0).rotation.y, 4);
        state.zRot = (float)System.Math.Round((decimal)transform.GetChild(0).rotation.z, 4);
        state.wRot = (float)System.Math.Round((decimal)transform.GetChild(0).rotation.w, 4);

        state.status = (byte)entityManager.networkAnimationStatus;
        if (UserDatas.is_guest)
        {
            string indentity = SystemInfo.deviceUniqueIdentifier;
            state.username = $"Guest#{indentity.Substring(indentity.Length - 4, 4)}";
        }
        else
        {
            if (string.IsNullOrEmpty(UserDatas.user_Data.info.username))
                state.username = $"User#{UserDatas.user_Data.info.address.Substring(UserDatas.user_Data.info.address.Length - 4, 4)}";
            else
                state.username = UserDatas.user_Data.info.username;
        }
        state.address = UserDatas.user_Data.info.address;
        state.character_id = (byte)(UserDatas.current_vehicle.is_equip ? int.Parse(UserDatas.current_vehicle.itemID) : -1);

        //No need to update again if last sent state == current view modified state
        if (localUpdatedState != null)
        {
            //TODO: Uses reflection so might be slow, replace with defined compare to improve speed
            List<NetworkedEntityChanges> changesLocal = NetworkedEntityChanges.Compare(localUpdatedState, state);
            int change_local_length = changesLocal.Count;
            if (change_local_length == 0 || (change_local_length == 1 && changesLocal[0].Name == "timestamp"))
            {
                return;
            }
        }
        // Debug.Log(new Vector3(state.xPos, state.yPos, state.zPos));

        //TODO: Uses reflection so might be slow, replace with defined compare to improve speed
        List<NetworkedEntityChanges> changes = NetworkedEntityChanges.Compare(previousState, state);
        int change_length = changes.Count;
        //Transform has been update locally, push changes
        if (change_length > 0)
        {
            //Create Change Set Array for NetSend
            ObscuredInt length = (change_length * 2) + 3;
            object[] changeSet = new object[length];
            changeSet[0] = state.id;
            int saveIndex = 1;
            for (int i = 0; i < change_length; i++)
            {
                changeSet[saveIndex] = changes[i].Name;
                changeSet[saveIndex + 1] = changes[i].NewValue;
                saveIndex += 2;
            }
            changeSet[saveIndex] = "visible_ow";
            changeSet[saveIndex + 1] = isVisibleOw;
            localUpdatedState = state.Clone();
            NetworkingManager.NetSendOpenworldRoom(EventName.entityUpdate, changeSet);
        }
    }
    private void OnStateChange(List<DataChange> changes)
    {
        if (entityManager != null)
            entityManager.SetName(state.username, state.address);
        //If not mine Sync
        if (!isMine)
        {
            SyncViewWithServer();
            stateMove = state.status;
            stateVehicle = state.character_id;
        }
        else
        {
            //hihi;
        }
    }
    private void SyncViewWithServer()
    {
        // Network player, receive data
        Vector3 pos = new Vector3((float)state.xPos, (float)state.yPos, (float)state.zPos) + new Vector3(0, 1.08f, 0);
        Quaternion rot = new Quaternion((float)state.xRot, (float)state.yRot, (float)state.zRot, (float)state.wRot);
        // Shift the buffer sideways, deleting state 20
        int proxy_states_length = proxyStates.Length;
        for (int i = proxy_states_length - 1; i >= 1; i--)
        {
            proxyStates[i] = proxyStates[i - 1];
        }

        // Record current state in slot 0
        EntityState newState = new EntityState() { timestamp = state.timestamp }; //Make sure timestamp is in ms
                                                                                  //newState.timestamp = state.timestamp;

        newState.pos = pos;
        newState.rot = rot;
        proxyStates[0] = newState;


        // Update used slot count, however never exceed the buffer size
        // Slots aren't actually freed so this just makes sure the buffer is
        // filled up and that uninitalized slots aren't used.
        proxyStateCount = Mathf.Min(proxyStateCount + 1, proxyStates.Length);

        // Check if states are in order
        if (proxyStates[0].timestamp < proxyStates[1].timestamp)
        {
#if UNITY_EDITOR
            Debug.Log("Timestamp inconsistent: " + proxyStates[0].timestamp + " should be greater than " + proxyStates[1].timestamp);
#endif
        }
    }
    protected virtual void ProcessViewSync()
    {

        //ProcessOtherPlayerMoving();

        if (ignoreMovementSync)
        {
            //Don't lerp this object right now
            return;
        }

        // This is the target playback time of this body
        ObscuredDouble interpolationTime = NetworkingManager.Instance.ServerTimeFake - interpolationBackTimeMs;
        // Debug.Log("interTime: " + interpolationTime);
        // Debug.Log("proxy Time: " + proxyStates[0].timestamp);
        // Use interpolation if the target playback time is present in the buffer
        if (proxyStates[0].timestamp > interpolationTime)
        {
            // The longer the time since last update add a delta factor to the lerp speed to get there quicker
            EntityState entity_0 = proxyStates[0];
            ObscuredFloat server_time = NetworkingManager.Instance.ServerTimeFake;
            ObscuredDouble proxy_states_0_time = entity_0.timestamp;
            ObscuredFloat deltaFactor = (server_time > proxy_states_0_time) ?
                (ObscuredFloat)(server_time - proxy_states_0_time) * 0.2f : 0f;
            ObscuredFloat distance = Vector3.Distance(transform.position, entity_0.pos);
            transform.position = Vector3.LerpUnclamped(transform.position, entity_0.pos, Time.deltaTime * (positionLerpSpeed + deltaFactor));
            transform.GetChild(0).rotation = entity_0.rot;
        }
        // Use extrapolation (If we did not get a packet in the last "X" ms and object had velocity)
        else
        {
            EntityState latest = proxyStates[0];

            ObscuredFloat extrapolationLength = (ObscuredFloat)(interpolationTime - latest.timestamp);
            // Don't extrapolate for more than 500 ms, you would need to do that carefully
            if (extrapolationLength < extrapolationLimitMs / 1000f)
            {
                transform.position = latest.pos;
                transform.GetChild(0).rotation = latest.rot;
            }
        }

    }
}
