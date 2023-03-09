using System;
using System.Collections;
using System.Collections.Generic;
using Colyseus;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[UIPanelPrefabAttr("PopupJoinClassRoom", "PopupCanvas")]
public class PopupJoinClassRoom : BasePanel
{
    [SerializeField]
    private TMP_InputField inputId, inputPass;
    [SerializeField]
    private Button btJoin, btClose;
    [SerializeField]
    private TextMeshProUGUI txErrorID, txErrorPass, txErrorJoin;
    public Action onDone = null;
    private bool isInputIdSelected = false;

    protected override void Awake()
    {
        base.Awake();
        if (btJoin != null)
            btJoin.onClick.AddListener(ClickJoin);
        if (btClose != null)
            btClose.onClick.AddListener(Close);
        if (inputId != null)
            inputId.onValueChanged.AddListener(ValidateId);
        if (inputPass != null)
            inputPass.onValueChanged.AddListener(ValidatePass);
    }

    private void ValidateId(string content)
    {
        if (inputId == null) return;
        if (string.IsNullOrEmpty(content)) return;
        if (content.Contains("\n"))
            content = content.Replace("\n", "");
        inputId.text = content.Trim();
    }

    private void ValidatePass(string content)
    {
        if (inputPass == null) return;
        if (string.IsNullOrEmpty(content)) return;
        if (content.Contains("\n"))
            content = content.Replace("\n", "");
        inputPass.text = content.Trim();
    }

    public void SetInputSelected(bool select)
    {
        isInputIdSelected = select;
    }

    private void Start()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.offsetMin = Vector2.zero;
    }

    private void OnEnable()
    {
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.Return, "ClickJoin", ClickJoin, ActionKeyType.Up);
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.KeypadEnter, "ClickJoin", ClickJoin, ActionKeyType.Up);
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.Tab, "JumpToInputField", JumpToInputField, ActionKeyType.Up);
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.Escape, "Close", Close, ActionKeyType.Up);
        Dictionary<KeyCode, List<string>> listAvailableKey = new Dictionary<KeyCode, List<string>>();
        List<string> return_action_names = new List<string>();
        return_action_names.Add("ClickJoin");
        listAvailableKey.Add(KeyCode.Return, return_action_names);
        listAvailableKey.Add(KeyCode.KeypadEnter, return_action_names);
        List<string> tab_action_names = new List<string>();
        tab_action_names.Add("JumpToInputField");
        listAvailableKey.Add(KeyCode.Tab, tab_action_names);
        List<string> esc_action_names = new List<string>();
        esc_action_names.Add("Close");
        listAvailableKey.Add(KeyCode.Escape, esc_action_names);
        InputRegisterEvent.Instance.SetlstKeyAvailable(listAvailableKey);
        GameConfig.gameBlockInput = true;
        Ultis.SetActiveCursor(true);
        Clear();
    }

    private void OnDestroy()
    {
        RemoveEvents();
    }

    private void Close()
    {
        RemoveEvents();
        GameConfig.gameBlockInput = false;
        Ultis.SetActiveCursor(false);
        onDone?.Invoke();
        HidePanel();
    }

    private void RemoveEvents()
    {
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.Return, "ClickJoin", ClickJoin, ActionKeyType.Up);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.KeypadEnter, "ClickJoin", ClickJoin, ActionKeyType.Up);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.Tab, "JumpToInputField", JumpToInputField, ActionKeyType.Up);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.Escape, "Close", Close, ActionKeyType.Up);
    }

    private void Clear()
    {
        if (inputId != null)
            inputId.text = "";
        if (inputPass != null)
            inputPass.text = null;
        SetText(txErrorID, "");
        SetText(txErrorPass, "");
        SetText(txErrorJoin, "");
    }

    private void ClickJoin()
    {
        string error = "";
        SetText(txErrorID, error);
        SetText(txErrorPass, error);
        string id = "";
        if (inputId != null)
            id = inputId.text;
        //id = "Class_1";
        string pass = "";
        if (inputPass != null)
            pass = inputPass.text;
        //pass = "1234561";
        if (string.IsNullOrEmpty(id))
        {
            error = "ID room can not be empty";
            Debug.LogError(error);
            SetText(txErrorID, error);
            return;
        }
        if (string.IsNullOrEmpty(pass))
        {
            error = "Pass room can not be empty";
            Debug.LogError(error);
            SetText(txErrorPass, error);
            return;
        }
        NetworkingManager.Instance.OnJoinClassRoomSuccess = delegate { LoadScene(id); };
        NetworkingManager.Instance.JoinClassRoom(id.Trim(), pass.Trim(), (error) =>
        {
            SetText(txErrorJoin, error);
        });
    }

    private void SetText(TextMeshProUGUI tx, string error)
    {
        if (tx != null)
            tx.text = error;
    }

    private void LoadScene(string current_class_id)
    {
        PopupChangeScene popupChangeScene = PanelManager.Show<PopupChangeScene>();
        onDone?.Invoke();
        string last_class = UserDatas.lastClassIdJoined;
        if (!last_class.Equals(current_class_id))
        {
            ImageManager.instance.DestroyTexturesUnuse(false);
            UserDatas.lastClassIdJoined = current_class_id;
        }
        ScenesManager.Instance.GetScene(BundleName.SCENES, AllSceneName.ClassRoom, false, delegate
        {
            ScenesManager.Instance.GetScene(BundleName.SCENES, AllSceneName.ClassRoomCanvas, true, delegate
            {
            }, true);
        }, true);
    }

    private void JumpToInputField()
    {
        isInputIdSelected = !isInputIdSelected;
        if (isInputIdSelected)
        {
            if (inputId != null)
            {
                inputId.Select();
                inputId.ActivateInputField();
            }
            if (inputPass != null)
            {
                inputPass.DeactivateInputField();
            }
        }
        else
        {
            if (inputPass != null)
            {
                inputPass.Select();
                inputPass.ActivateInputField();
            }
            if (inputId != null)
            {
                inputId.DeactivateInputField();
            }

        }
    }
}
