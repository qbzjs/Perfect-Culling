using System;
using System.Collections;
using System.Collections.Generic;
using Colyseus;
using CodeStage.AntiCheat.Storage;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class CanvasManager : SingletonMono<CanvasManager>
{
    [SerializeField]
    private Camera _canvasCamera;
    public Camera canvasCamera => _canvasCamera;
    [SerializeField]
    private Button btCloseChat, btChat;
    [SerializeField]
    private bool _isClickButtonChat = false;
    private bool _isstatusbutton = false;
    MainMenuManager mainMenu = null;
    private PopupChat _popupChat = null;
    private PopupChat popupChat
    {
        get
        {
            if (_popupChat == null)
                _popupChat = PanelManager.Show<PopupChat>();
            return _popupChat;
        }
    }
    [SerializeField] private Button btMainMenu;
    private Animator _animatorPopUpWheel;
    private Animator animatorPopUpWheel
    {
        get
        {
            if (_animatorPopUpWheel == null) _animatorPopUpWheel = btWheel.GetComponent<Animator>();
            return _animatorPopUpWheel;
        }
    }
    [SerializeField]
    private ButtonWheel btWheel;
    private PopupPlayerInfo popupPlayerInfo = null;
    private PopUpKeyControlManager popUpKeyControlManager = null;
    private ColyseusRoom<OWRoomState> openworldRoom;
    [SerializeField] private Button btQuest;

    protected override void Awake()
    {
        base.Awake();
        StopAllCoroutines();
        Observer.Instance.AddObserver(ObserverKey.Teleport, TeleportEffect);
        Observer.Instance.AddObserver(ObserverKey.MapTeleport, OnMapTeleport);
        Observer.Instance.AddObserver(ObserverKey.RayCastHitObject, SetActivePopupInteract);
        Observer.Instance.AddObserver(ObserverKey.UpdateBtWheelState, UpdateBtWheelState);
        Observer.Instance.AddObserver(ObserverKey.SetActiveMissionMain, SetStatusButtonQuest);
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.Return, "ActivePopUpChat", ActivePopUpChat, ActionKeyType.Up);
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.T, "ActivePopupEmotion", ActivePopupEmotion, ActionKeyType.Down);
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.M, "SetActiveMap", SetActiveMap, ActionKeyType.Down);
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.Escape, "SetActiveMainMenu", SetActiveMainMenu, ActionKeyType.Down);
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.F1, "SetStatusKeyControl", SetStatusKeyControl, ActionKeyType.Up);
        // InputRegisterEvent.Instance.RegisterEvent(KeyCode.P, OpenSelectCharacter, ActionKeyType.Up);
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.F2, "SetActiveChangeLog", SetActiveChangeLog, ActionKeyType.Up);
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.Tab, "OpenPopupPlayerInfo", OpenPopupPlayerInfo, ActionKeyType.Down);
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.Tab, "ClosePopupPlayerInfo", ClosePopupPlayerInfo, ActionKeyType.Up);
        UserDatas.record_chat_history = null;
        NetworkingManager.OnMessage<RecordChatHistory>(EventName.GET_CHAT_HISTORY, (msg) =>
        {
            UserDatas.record_chat_history = msg.chat_histories.ToArray();
            popupChat.CreateItemsFromServer(UserDatas.record_chat_history);
        });
        NetworkingManager.NetSend(EventName.GET_CHAT_HISTORY, null);

    }

    protected override void OnDestroy()
    {
        StopAllCoroutines();
        Observer.Instance.RemoveObserver(ObserverKey.Teleport, TeleportEffect);
        Observer.Instance.RemoveObserver(ObserverKey.MapTeleport, OnMapTeleport);
        Observer.Instance.RemoveObserver(ObserverKey.RayCastHitObject, SetActivePopupInteract);
        Observer.Instance.RemoveObserver(ObserverKey.UpdateBtWheelState, UpdateBtWheelState);
        Observer.Instance.RemoveObserver(ObserverKey.SetActiveMissionMain, SetStatusButtonQuest);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.Return, "ActivePopUpChat", ActivePopUpChat, ActionKeyType.Up);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.F1, "SetStatusKeyControl", SetStatusKeyControl, ActionKeyType.Up);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.T, "ActivePopupEmotion", ActivePopupEmotion, ActionKeyType.Down);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.M, "SetActiveMap", SetActiveMap, ActionKeyType.Down);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.Escape, "SetActiveMainMenu", SetActiveMainMenu, ActionKeyType.Down);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.F2, "SetActiveChangeLog", SetActiveChangeLog, ActionKeyType.Up);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.Tab, "OpenPopupPlayerInfo", OpenPopupPlayerInfo, ActionKeyType.Down);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.Tab, "ClosePopupPlayerInfo", ClosePopupPlayerInfo, ActionKeyType.Up);
        UnregisterOpenworldRoomEvents();
        // InputRegisterEvent.Instance.RemoveEventKey(KeyCode.P, OpenSelectCharacter, ActionKeyType.Up);
        base.OnDestroy();
    }

    private void Start()
    {
        if (btCloseChat != null)
            btCloseChat.onClick.AddListener(CloseChat);
        if (btChat != null)
        {
            btChat.onClick.AddListener(OnClickButtonChat);
        }

        if (btMainMenu != null)
        {
            btMainMenu.onClick.AddListener(OnClickActiveMainMenu);
        }
        if (btWheel != null)
        {
            btWheel.GetComponent<Button>().onClick.AddListener(SetStatusButtonWheel);
        }
        OpenPopupTeleport("The Headquarter");
        IsCompleteTutorial();
        PanelManager.Show<PopupChangeScene>().Hide();
        StartCoroutine(IEInit(delegate
        {
            RegisterOpenworldRoomEvents();
        }));
        SetStatusButtonQuest();
    }

    private void SetStatusButtonQuest(object data= null)
    {
        if (btQuest != null && UserDatas.user_Data.info.is_tutorial_done)
        {
            btQuest.gameObject.SetActive(true);
            btQuest.onClick.AddListener(ClickBtQuest);
        }
        else
        {
            btQuest.gameObject.SetActive(false);
        }
    }
    private void ClickBtQuest()
    {
        PanelManager.Show<MissionInfoManager>();
    }

    private void IsCompleteTutorial()
    {
        bool isComplete = UserDatas.user_Data.info.is_tutorial_done;
        if (!isComplete)
        {
            ShowTutorialFirstPlay();
        }
    }

    private IEnumerator IEInit(UnityAction action)
    {
        yield return new WaitUntil(() => NetworkingManager.Instance.openworldRoom != null);
        action?.Invoke();
    }

    private void RegisterOpenworldRoomEvents()
    {
        openworldRoom = NetworkingManager.Instance.openworldRoom;
        if (openworldRoom != null)
        {
            openworldRoom.State.users.OnAdd += NetworkedUsers_OnAdd;
            openworldRoom.State.users.OnRemove += NetworkedUsers_OnRemove;
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
            openworldRoom.State.users.OnAdd -= NetworkedUsers_OnAdd;
            openworldRoom.State.users.OnRemove -= NetworkedUsers_OnRemove;
        }
        else
        {
            Debug.LogError($"Cannot unregister room handlers, room is null!");
        }
    }

    private void SetStatusKeyControl()
    {
        if (popUpKeyControlManager == null || !popUpKeyControlManager.gameObject.activeSelf)
        {
            ActiveKeyControl();
        }
        else
        {
            DeActiveKeyControl();
        }
    }
    private void ActiveKeyControl()
    {
        popUpKeyControlManager = PanelManager.Show<PopUpKeyControlManager>();
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.Escape, "SetActiveMainMenu", SetActiveMainMenu, ActionKeyType.Down);
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.Escape, "DeActiveKeyControl", DeActiveKeyControl, ActionKeyType.Down);
        popUpKeyControlManager.action = () => { InputRegisterEvent.Instance.RegisterEvent(KeyCode.Escape, "SetActiveMainMenu", SetActiveMainMenu, ActionKeyType.Down); };
    }
    private void DeActiveKeyControl()
    {
        PanelManager.Hide<PopUpKeyControlManager>();
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.Escape, "SetActiveMainMenu", SetActiveMainMenu, ActionKeyType.Down);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.Escape, "DeActiveKeyControl", DeActiveKeyControl, ActionKeyType.Down);
    }


    private void OnClickActiveMainMenu()
    {
        SetActiveMainMenu();
    }
    private void SetActiveMainMenu()
    {
        if (mainMenu == null || !mainMenu.gameObject.activeSelf)
        {
            mainMenu = PanelManager.Show<MainMenuManager>();
            Ultis.SetActiveCursor(true);
            GameConfig.gameBlockInput = true;

            Dictionary<KeyCode, List<string>> listAvailableKey = new Dictionary<KeyCode, List<string>>();
            List<string> escape_action_names = new List<string>();
            escape_action_names.Add("SetActiveMainMenu");
            listAvailableKey.Add(KeyCode.Escape, escape_action_names);
            listAvailableKey.Add(KeyCode.Mouse0, null);
            InputRegisterEvent.Instance.SetlstKeyAvailable(listAvailableKey);
            PanelManager.Hide<PopupInteract>();
        }
        else
        {
            Ultis.SetActiveCursor(false);
            GameConfig.gameBlockInput = false;
            PanelManager.Hide<MainMenuManager>();
        }
    }

    private TutorialFirstPlay tuto;
    private void ShowTutorialFirstPlay()
    {
        if (!PlayerPrefs.HasKey("tutorial_done"))
        {
            tuto = PanelManager.Show<TutorialFirstPlay>();
            tuto.StateStepTutorial(0);
        }
    }
    private void OnClickButtonChat()
    {
        if (popupChat == null) return;
        if (popupChat.isOpen == false)
            ActivePopUpChat();
        else
            CloseChat();
    }
    ChangeLogManager changeLogManager = null;
    private void SetActiveChangeLog()
    {
        if (changeLogManager == null || changeLogManager.gameObject.activeSelf == false)
        {
            changeLogManager = PanelManager.Show<ChangeLogManager>();
            Ultis.SetActiveCursor(true);
            GameConfig.gameBlockInput = true;
            Dictionary<KeyCode, List<string>> listAvailableKey = new Dictionary<KeyCode, List<string>>();
            List<string> f2_action_names = new List<string>();
            f2_action_names.Add("SetActiveChangeLog");
            listAvailableKey.Add(KeyCode.Mouse0, null);
            listAvailableKey.Add(KeyCode.F2, f2_action_names);
            InputRegisterEvent.Instance.SetlstKeyAvailable(listAvailableKey);
        }
        else
        {
            Ultis.SetActiveCursor(false);
            GameConfig.gameBlockInput = false;
            PanelManager.Hide<ChangeLogManager>();
        }
    }
    private void ActivePopUpChat()
    {
        if (popupChat == null) return;
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.Escape, "SetActiveMainMenu", SetActiveMainMenu, ActionKeyType.Down);
        popupChat.actionStartOpened = OnStartChatOpened;
        popupChat.actionEndClosed = OnEndChatClosed;
        if (popupChat.isOpen == false)
        {
            popupChat.SetStatusPopUp(true);
            SetActiveBtCloseChat(true);
        }
        else
            popupChat.SelectTextInChat();
    }

    private void CloseChat()
    {
        if (popupChat == null) return;
        Ultis.SetActiveCursor(false);
        popupChat.SetStatusPopUp(false);
    }
    private void ActivePopupEmotion()
    {
        Ultis.SetActiveCursor(true);
        Dictionary<KeyCode, List<string>> listAvailableKey = new Dictionary<KeyCode, List<string>>();
        listAvailableKey.Add(KeyCode.T, null);
        InputRegisterEvent.Instance.SetlstKeyAvailable(listAvailableKey);
        GameConfig.gameBlockInput = true;
        PanelManager.Show<PopupEmotion>();

    }
    private void OnMapTeleport(object data)
    {
        SetActiveMap();
        string placeName = (string)data;
        OpenPopupTeleport(placeName);
    }
    private void OpenPopupTeleport(string place_name)
    {
        PopupTeleportEffect popupTeleportEffect = PanelManager.Show<PopupTeleportEffect>();
        popupTeleportEffect.SetPlaceName(place_name);
        popupTeleportEffect.PlayAnim();
    }
    private void SetActiveMap()
    {
        Minimap minimap = PanelManager.Show<Minimap>();
        minimap.SetActiveMap();
    }
    PopupInteract popUpInteract;
    private void SetActivePopupInteract(object data)
    {
        ResponseInteraction [] responseInteraction = (ResponseInteraction[] )data;
        if (responseInteraction != null)
        {
            PanelManager.Hide<PopupInteract>();
            popUpInteract = PanelManager.Show<PopupInteract>();
            popUpInteract.SetInteractionType(responseInteraction);

        }
        else if (popUpInteract != null && popUpInteract.gameObject.activeSelf)
        {
            PanelManager.Hide<PopupInteract>();
        }
    }
    private void SetActiveBtCloseChat(bool active)
    {
        if (btCloseChat != null)
            btCloseChat.gameObject.SetActive(active);
    }

    private void TeleportEffect(object data)
    {
        PanelManager.Show<PopupTeleportEffect>();
    }

    private void OnStartChatOpened()
    {
        StatusAnimationWheel(true);
    }
    private void OnEndChatClosed()
    {
        StatusAnimationWheel(false);
        SetActiveBtCloseChat(false);
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.Escape, "SetActiveMainMenu", SetActiveMainMenu, ActionKeyType.Down);
    }



    private void OpenSelectCharacter()
    {
        Ultis.SetActiveCursor(true);
        PopupSelectCharacter popupSelectCharacter = PanelManager.Show<PopupSelectCharacter>();
        popupSelectCharacter.onClose = CloseSelectCharacter;
        //InputRegisterEvent.Instance.RemoveEventKey(KeyCode.P, OpenSelectCharacter, ActionKeyType.Up);
        //InputRegisterEvent.Instance.RegisterEvent(KeyCode.P, CloseSelectCharacter, ActionKeyType.Up);
    }

    private void CloseSelectCharacter()
    {
        Ultis.SetActiveCursor(false);
        PanelManager.Hide<PopupSelectCharacter>();
        //InputRegisterEvent.Instance.RemoveEventKey(KeyCode.P, CloseSelectCharacter, ActionKeyType.Up);
        //InputRegisterEvent.Instance.RegisterEvent(KeyCode.P, OpenSelectCharacter, ActionKeyType.Up);
    }

    private void SetStatusButtonWheel()
    {
        Observer.Instance.Notify(ObserverKey.SetStatusEquipVehicle);
    }

    private void UpdateBtWheelState(object data)
    {
        if (data == null) return;
        bool active = (bool)data;
        btWheel.SetStatusLight(active);
    }

    private void StatusAnimationWheel(bool status)
    {
        animatorPopUpWheel.SetBool("statuschat", status);
    }

    private void OpenPopupPlayerInfo()
    {
        if (popupPlayerInfo == null)
        {
            List<NetworkedEntityState> networkedEntityStates = new List<NetworkedEntityState>();
            openworldRoom.State.users.ForEach((key, value) =>
            {
                networkedEntityStates.Add(value);
            });
            popupPlayerInfo = PanelManager.Show<PopupPlayerInfo>();
            popupPlayerInfo.Init(networkedEntityStates);
        }
        else
        {
            popupPlayerInfo = PanelManager.Show<PopupPlayerInfo>();
        }
    }
    private void ClosePopupPlayerInfo()
    {
        PanelManager.Hide<PopupPlayerInfo>();
    }

    private void NetworkedUsers_OnAdd(string key, NetworkedEntityState value)
    {
        if (popupPlayerInfo != null)
        {
            popupPlayerInfo.OnAddUser(key, value);
        }
    }
    private void NetworkedUsers_OnRemove(string key, NetworkedEntityState value)
    {
        if (popupPlayerInfo != null)
        {
            popupPlayerInfo.OnRemoveUser(key, value);
        }
    }
}
