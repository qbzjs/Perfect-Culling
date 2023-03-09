using System;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using Colyseus;
using UnityEngine;
using UnityEngine.Events;

public class NetworkingManager : ColyseusManager<NetworkingManager>
{
    public Action OnJoinOpenworldSuccess = null;
    public Action OnJoinOpenworldFailed = null;
    private ColyseusRoom<OWRoomState> _openworldRoom;
    public ColyseusRoom<OWRoomState> openworldRoom
    {
        get
        {
            return _openworldRoom;
        }

        private set
        {
            _openworldRoom = value;
        }
    }

    public Action OnJoinClassRoomSuccess = null;
    private ColyseusRoom<ClassRoomState> _classRoom;
    public ColyseusRoom<ClassRoomState> classRoom
    {
        get
        {
            return _classRoom;
        }

        private set
        {
            _classRoom = value;
        }
    }

    private void Stop()
    {
        StopAllCoroutines();
        openworldRoom?.Leave(true);
        openworldRoom = null;
        classRoom?.Leave(true);
        classRoom = null;
    }

    protected override void OnDestroy()
    {
        Stop();
        base.OnDestroy();
    }

    protected override void OnApplicationQuit()
    {
        Stop();
        base.OnApplicationQuit();
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }


    protected override void Start()
    {
        base.Start();
        InitializeClient();
        client.Settings.colyseusServerAddress = EnvironmentConfig.linkServerAddress;
        client.Settings.useSecureProtocol = true;
        InvokeRepeating("Ping", 0, 30);
    }
    long currentTimeStamp;
    private void Ping()
    {
        currentTimeStamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        NetSendOpenworldRoom(EventName.PING, currentTimeStamp);
    }

    public static void NetSend(ObscuredString action, object message = null)
    {
        CurrentRoomType currentRoomType = UserDatas.current_room_type;
        switch (currentRoomType)
        {
            case CurrentRoomType.Openworld:
                NetSendOpenworldRoom(action, message);
                break;
            case CurrentRoomType.Class:
                NetSendClassRoom(action, message);
                break;
            default:
                break;
        }
    }

    public static void NetSendOpenworldRoom(ObscuredString action, object message = null)
    {
        if (Instance.openworldRoom == null)
        {
            //Debug.LogError($"Error: Not in custom room for action {action} msg {message}");
            Instance.StartCoroutine(Instance.IEWaitOpenworldRoomInit(action, message));
            return;
        }
        _ = message == null ? Instance.openworldRoom.Send(action) : Instance.openworldRoom.Send(action, message);
    }

    private IEnumerator IEWaitOpenworldRoomInit(ObscuredString action, object message = null)
    {
        yield return new WaitUntil(() => openworldRoom != null);
        _ = message == null ? Instance.openworldRoom.Send(action) : Instance.openworldRoom.Send(action, message);
    }

    private static void NetSendClassRoom(ObscuredString action, object message = null)
    {
        if (Instance.classRoom == null)
        {
            //Debug.LogError($"Error: Not in custom room for action {action} msg {message}");
            Instance.StartCoroutine(Instance.IEWaitClassRoomInit(action, message));
            return;
        }
        _ = message == null ? Instance.classRoom.Send(action) : Instance.classRoom.Send(action, message);
    }

    private IEnumerator IEWaitClassRoomInit(ObscuredString action, object message = null)
    {
        yield return new WaitUntil(() => classRoom != null);
        _ = message == null ? Instance.classRoom.Send(action) : Instance.classRoom.Send(action, message);
    }

    public static void OnMessage<MessageType>(ObscuredString type, Action<MessageType> handler)
    {
        CurrentRoomType currentRoomType = UserDatas.current_room_type;
        switch (currentRoomType)
        {
            case CurrentRoomType.Openworld:
                Instance.openworldRoom.OnMessage(type, handler);
                break;
            case CurrentRoomType.Class:
                Instance.classRoom.OnMessage(type, handler);
                break;
            default:
                break;
        }
    }

    public static void OnRemoveMessage(ObscuredString type)
    {
        CurrentRoomType currentRoomType = UserDatas.current_room_type;
        switch (currentRoomType)
        {
            case CurrentRoomType.Openworld:
                if (Instance.openworldRoom != null)
                    Instance.openworldRoom.RemoveMessage(type);
                break;
            case CurrentRoomType.Class:
                if (Instance.classRoom != null)
                    Instance.classRoom.RemoveMessage(type);
                break;
            default:
                break;
        }

    }

    public void JoinOpenworldRoom()
    {
        try
        {
            TPRLAPI.instance.JoinOpenworldRoom((data) =>
            {
                RecordJoinOpenworldRoomResponse response = data;
                ColyseusRoomAvailable availableRoom = new ColyseusRoomAvailable();
                if (!response.error)
                {
                    availableRoom.clients = (uint)response.output.seatReservation.room.clients;
                    availableRoom.maxClients = (uint)response.output.seatReservation.room.maxClients;
                    availableRoom.name = response.output.seatReservation.room.name;
                    availableRoom.processId = response.output.seatReservation.room.processId;
                    availableRoom.roomId = response.output.seatReservation.room.roomId;
                    string sessionId = response.output.seatReservation.sessionId;
                    UserDatas.ow_session_id = sessionId;
                    ConsumeSeatOpenworldSeatReservation(availableRoom, sessionId);
                }
                else
                    OnJoinOpenworldFailed?.Invoke();
            });
        }
        catch (System.Exception error)
        {
            Debug.LogError(error);
        }
    }

    public async void ConsumeSeatOpenworldSeatReservation(ColyseusRoomAvailable room, string session_id)
    {
        try
        {
            ColyseusMatchMakeResponse response = new ColyseusMatchMakeResponse() { room = room, sessionId = session_id };
            openworldRoom = await client.ConsumeSeatReservation<OWRoomState>(response);
            UserDatas.current_room_type = CurrentRoomType.Openworld;
            OnJoinOpenworldSuccess?.Invoke();
            InvokeRepeating("FakeServerTime", 0, 1);
        }
        catch (System.Exception error)
        {
            //TODO: Display popup here
            Debug.LogError(error);
        }
    }

    private void FakeServerTime()
    {
        fake_server_time += 1;
    }

    private ObscuredFloat fake_server_time = 0;
    public ObscuredFloat ServerTimeFake
    {
        get
        {
            return fake_server_time;
        }
    }

    public void JoinClassRoom(string id, string pass, UnityAction<string> on_error)
    {
        try
        {
            TPRLAPI.instance.JoinClassRoom(id, pass, (data) =>
            {
                RecordJoinClassRoomResponse response = data;
                ColyseusRoomAvailable availableRoom = new ColyseusRoomAvailable();
                if (!response.error)
                {
                    availableRoom.clients = (uint)response.output.seatReservation.room.clients;
                    availableRoom.maxClients = (uint)response.output.seatReservation.room.maxClients;
                    availableRoom.name = response.output.seatReservation.room.name;
                    availableRoom.processId = response.output.seatReservation.room.processId;
                    availableRoom.roomId = response.output.seatReservation.room.roomId;
                    string sessionId = response.output.seatReservation.sessionId;
                    ConsumeSeatClassRoomReservation(availableRoom, sessionId);

                }
            }, on_error);
        }
        catch (System.Exception error)
        {
            Debug.LogError(error);
        }
    }

    public async void ConsumeSeatClassRoomReservation(ColyseusRoomAvailable room, string session_id)
    {
        try
        {
            ColyseusMatchMakeResponse response = new ColyseusMatchMakeResponse() { room = room, sessionId = session_id };
            classRoom = await client.ConsumeSeatReservation<ClassRoomState>(response);
            UserDatas.current_room_type = CurrentRoomType.Class;
            OnJoinClassRoomSuccess?.Invoke();
        }
        catch (System.Exception error)
        {
            //TODO: Display popup here
            Debug.LogError(error);
        }
    }
}
