using System;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[UIPanelPrefabAttr("PopupChat", "PopupChatParent")]
public class PopupChat : BasePanel
{
    [SerializeField] TMP_InputField inputFieldChat;
    [SerializeField] GameObject messageItem;
    [SerializeField] Transform parentInstantate;
    [SerializeField] GameObject chatFieldShow;
    [SerializeField] GameObject inputFieldChatText;
    private bool _isOpen = false;
    public bool isOpen => _isOpen;
    private string textInputChat;
    private Animator _animator;
    private Animator animator
    {
        get
        {

            if (_animator == null) _animator = GetComponent<Animator>();
            return _animator;
        }
    }
    public UnityAction actionEndClosed;
    public UnityAction actionStartOpened;


    protected override void Awake()
    {
        NetworkingManager.OnMessage<RecordChat>(EventName.NEW_CHAT_MESSAGE, (msg) =>
        {
            CreateItem(msg, isMine(msg));
        });
    }

    protected void OnEnable()
    {

    }

    private void OnDestroy()
    {
        NetworkingManager.OnRemoveMessage(EventName.NEW_CHAT_MESSAGE);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.Return, "CreateMessage", CreateMessage, ActionKeyType.Up);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.KeypadEnter, "CreateMessage", CreateMessage, ActionKeyType.Up);
    }

    private void ShowChatField()
    {
        if (chatFieldShow.activeSelf == false)
        {
            chatFieldShow.SetActive(true);
        }
    }

    public void SetStatusPopUp(bool active)
    {
        _isOpen = active;
        animator.SetBool("statuschat", _isOpen);
        Ultis.SetActiveCursor(active);
        GameConfig.gameBlockInput = active;
        if (_isOpen)
        {
            SelectTextInChat();
            Dictionary<KeyCode, List<string>> listAvailableKey = new Dictionary<KeyCode, List<string>>();
            List<string> return_action_names = new List<string>();
            return_action_names.Add("CreateMessage");
            listAvailableKey.Add(KeyCode.Return, return_action_names);
            listAvailableKey.Add(KeyCode.KeypadEnter, return_action_names);
            List<string> esc_action_names = new List<string>();
            esc_action_names.Add("DeActivePopUp");
            listAvailableKey.Add(KeyCode.Escape, esc_action_names);
            InputRegisterEvent.Instance.SetlstKeyAvailable(listAvailableKey);
            InputRegisterEvent.Instance.RegisterEvent(KeyCode.Return, "CreateMessage", CreateMessage, ActionKeyType.Up);
            InputRegisterEvent.Instance.RegisterEvent(KeyCode.KeypadEnter, "CreateMessage", CreateMessage, ActionKeyType.Up);
            InputRegisterEvent.Instance.RegisterEvent(KeyCode.Escape, "DeActivePopUp", DeActivePopUp, ActionKeyType.Up);
        }
        else
        {
            if (inputFieldChat != null)
            {
                inputFieldChat.text = "";
                inputFieldChat.DeactivateInputField();
            }
            InputRegisterEvent.Instance.RemoveEventKey(KeyCode.Return, "CreateMessage", CreateMessage, ActionKeyType.Up);
            InputRegisterEvent.Instance.RemoveEventKey(KeyCode.KeypadEnter, "CreateMessage", CreateMessage, ActionKeyType.Up);
            InputRegisterEvent.Instance.RemoveEventKey(KeyCode.Escape, "DeActivePopUp", DeActivePopUp, ActionKeyType.Up);
        }
    }
    public void SelectTextInChat()
    {
        if (inputFieldChat != null)
        {

            inputFieldChat.ActivateInputField();
            inputFieldChat.Select();
        }

    }
    private void DeActivePopUp()
    {
        SetStatusPopUp(false);
    }
    private void CreateMessage()
    {
        if (_isOpen == false) return;
        if (inputFieldChat != null)
        {
            textInputChat = inputFieldChat.text;
            if (string.IsNullOrEmpty(textInputChat)) {
                DeActivePopUp();
                return;
            }
            NetworkingManager.NetSend(EventName.NEW_CHAT_MESSAGE, textInputChat);
            inputFieldChat.text = "";
        }
        SelectTextInChat();
    }
    public void CreateItemsFromServer(RecordChat[] recordchats)
    {
        if (messageItem == null || recordchats == null) return;
        int length = recordchats.Length;
        for (int i = length - 1; i >= 0; i--)
        {
            RecordChat recordChat = recordchats[i];
            CreateItem(recordChat, isMine(recordChat));
        }
    }
    private void CreateItem(RecordChat recordchat, bool isMine)
    {
        if (messageItem != null)
        {
            ItemMessageChat chats = CreateController.instance.CreateObjectGetComponent<ItemMessageChat>(messageItem, Vector3.zero, parentInstantate);
            chats.SetInfor(recordchat, isMine);
        }
    }

    private bool isMine(RecordChat record)
    {
        string indentity = SystemInfo.deviceUniqueIdentifier;
        return record.s_id.Equals(UserDatas.user_Data.info.username)
                || record.s_id.Equals($"User#{UserDatas.user_Data.info.address.Substring(UserDatas.user_Data.info.address.Length - 4, 4)}") ||
                record.s_id.Equals($"Guest#{indentity.Substring(indentity.Length - 4, 4)}");
    }

    public void OnEndClose()
    {
        actionEndClosed?.Invoke();
    }
    public void OnStartOpen()
    {
        actionStartOpened?.Invoke();
    }
    private void OnInputFieldChat()
    {
        inputFieldChatText.SetActive(true);
    }
    private void OffInputFieldChat()
    {
        inputFieldChatText.SetActive(false);
    }

}
