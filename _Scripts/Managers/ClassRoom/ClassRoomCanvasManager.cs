using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClassRoomCanvasManager : SingletonMono<ClassRoomCanvasManager>
{
    [SerializeField]
    private Camera _canvasCamera;
    public Camera canvasCamera => _canvasCamera;
    [SerializeField]
    private Button btCloseChat, btChat;
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
    [SerializeField] private Animator animatorPopUpWheel;
    private void StatusAnimationWheel(bool status)
    {
        animatorPopUpWheel.SetBool("statuschat", status);
    }

    protected override void Awake()
    {
        base.Awake();
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.Return, "ActivePopUpChat", ActivePopUpChat, ActionKeyType.Up);
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.T, "ActivePopupEmotion", ActivePopupEmotion, ActionKeyType.Down);
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.F1, "SetStatusKeyControl", SetStatusKeyControl, ActionKeyType.Up);
        Observer.Instance.AddObserver(ObserverKey.RayCastHitObject, SetActivePopupInteract);
        UserDatas.record_chat_history = null;
        NetworkingManager.OnMessage<RecordChatHistory>(EventName.GET_CHAT_HISTORY, (msg) =>
        {
            UserDatas.record_chat_history = msg.chat_histories.ToArray();
            popupChat.CreateItemsFromServer(UserDatas.record_chat_history);
        });
        NetworkingManager.NetSend(EventName.GET_CHAT_HISTORY, null);
        Observer.Instance.AddObserver(ObserverKey.ClassRoomUpdateUI, ShowTutorialRemote);
    }

    protected override void OnDestroy()
    {
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.Return, "ActivePopUpChat", ActivePopUpChat, ActionKeyType.Up);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.T, "ActivePopupEmotion", ActivePopupEmotion, ActionKeyType.Down);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.F1, "SetStatusKeyControl", SetStatusKeyControl, ActionKeyType.Up);
        Observer.Instance.RemoveObserver(ObserverKey.RayCastHitObject, SetActivePopupInteract);
        Observer.Instance.RemoveObserver(ObserverKey.ClassRoomUpdateUI, ShowTutorialRemote);
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

           
    }

    private void ShowTutorialRemote(object data)
    {
        if (data != null)
        {
            ClassRoomRole role = (ClassRoomRole)data;
            if (role == ClassRoomRole.teacher)
            {
               PanelManager.Show<TutorialRemoteClassManager>();
            }

        }
    }
    private PopUpKeyControlManager popUpKeyControlManager = null;
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
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.Escape, "DeActiveKeyControl", DeActiveKeyControl, ActionKeyType.Down);
        popUpKeyControlManager.action = null;
    }
    private void DeActiveKeyControl()
    {
        PanelManager.Hide<PopUpKeyControlManager>();
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.Escape, "DeActiveKeyControl", DeActiveKeyControl, ActionKeyType.Down);
    }


    PopupInteract popUpInteract;
    private void SetActivePopupInteract(object data)
    {
        ResponseInteraction [] responseInteraction = (ResponseInteraction[] )data;
        if (responseInteraction != null)
        {
            popUpInteract = PanelManager.Show<PopupInteract>();
            popUpInteract.SetInteractionType(responseInteraction);
        }
        else if (popUpInteract != null && popUpInteract.gameObject.activeSelf)
        {
            PanelManager.Hide<PopupInteract>();
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

    private void ActivePopUpChat()
    {
        if (popupChat == null) return;
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
    private void OnStartChatOpened()
    {
        StatusAnimationWheel(true);
    }
    private void OnEndChatClosed()
    {
        SetActiveBtCloseChat(false);
        StatusAnimationWheel(false);
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

    private void SetActiveBtCloseChat(bool active)
    {
        if (btCloseChat != null)
            btCloseChat.gameObject.SetActive(active);
    }
}
