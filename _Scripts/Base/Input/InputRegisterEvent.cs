using CodeStage.AntiCheat.ObscuredTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputRegisterEvent : TPRLSingleton<InputRegisterEvent>
{
    private Dictionary<string, UnityAction> dicKeyActions = new Dictionary<string, UnityAction>();
    private Dictionary<string, UnityAction> dicKeyUpActions = new Dictionary<string, UnityAction>();
    private Dictionary<string, UnityAction> dicKeyDownActions = new Dictionary<string, UnityAction>();
    private Dictionary<KeyCode, List<string>> lstKeyActions = new Dictionary<KeyCode, List<string>>();
    private Dictionary<KeyCode, List<string>> lstKeyUpActions = new Dictionary<KeyCode, List<string>>();
    private Dictionary<KeyCode, List<string>> lstKeyDownActions = new Dictionary<KeyCode, List<string>>();
    private Dictionary<KeyCode, List<string>> lstKeyAvailable = new Dictionary<KeyCode, List<string>>();
    private bool isBlockInput = false;

    protected override void Awake()
    {
        dontDestroyOnLoad = true;
        base.Awake();
        Observer.Instance.AddObserver(ObserverKey.GameBlockKeyboard, BlockInput);
    }

    protected override void OnDestroy()
    {
        ClearAllEvents();
        Observer.Instance.RemoveObserver(ObserverKey.GameBlockKeyboard, BlockInput);
        base.OnDestroy();
    }

    public void RegisterEvent(KeyCode key, string action_name, UnityAction action, ActionKeyType key_type)
    {
        Dictionary<KeyCode, List<string>> action_names = GetListKeyEventByType(key_type);
        if (action_names == null) return;
        Dictionary<string, UnityAction> actions = GetDicEventByType(key_type);
        if (actions.ContainsKey(action_name))
            actions[action_name] = action;
        else
            actions.Add(action_name, action);
        List<string> act = null;
        if (action_names.ContainsKey(key))
        {
            act = action_names[key];
            if (act == null)
                act = new List<string>();
            if (!act.Contains(action_name))
                act.Add(action_name);
            action_names[key] = act;
            return;
        }
        act = new List<string>();
        act.Add(action_name);
        action_names.Add(key, act);

    }
    public void RemoveEventKey(KeyCode key, string action_name, UnityAction action, ActionKeyType key_type)
    {
        Dictionary<KeyCode, List<string>> action_names = GetListKeyEventByType(key_type);
        if (action_names == null) return;
        if (action_names.ContainsKey(key))
        {
            List<string> act = action_names[key];
            if (act.Contains(action_name))
                act.Remove(action_name);
            action_names[key] = act;
        }
        Dictionary<string, UnityAction> actions = GetDicEventByType(key_type);
        if (actions.ContainsKey(action_name))
            actions.Remove(action_name);
    }

    private Dictionary<KeyCode, List<string>> GetListKeyEventByType(ActionKeyType key_type)
    {
        Dictionary<KeyCode, List<string>> actions = null;
        switch (key_type)
        {
            case ActionKeyType.Down:
                actions = lstKeyDownActions;
                break;
            case ActionKeyType.Stay:
                actions = lstKeyActions;
                break;
            case ActionKeyType.Up:
                actions = lstKeyUpActions;
                break;
        }
        return actions;
    }

    private Dictionary<string, UnityAction> GetDicEventByType(ActionKeyType key_type)
    {
        Dictionary<string, UnityAction> actions = null;
        switch (key_type)
        {
            case ActionKeyType.Down:
                actions = dicKeyDownActions;
                break;
            case ActionKeyType.Stay:
                actions = dicKeyActions;
                break;
            case ActionKeyType.Up:
                actions = dicKeyUpActions;
                break;
        }
        return actions;
    }

    private void SetListKeyEventByType(Dictionary<KeyCode, List<string>> actions, ActionKeyType key_type)
    {
        switch (key_type)
        {
            case ActionKeyType.Down:
                lstKeyDownActions = actions;
                break;
            case ActionKeyType.Stay:
                lstKeyActions = actions;
                break;
            case ActionKeyType.Up:
                lstKeyUpActions = actions;
                break;
        }
    }

    private void BlockInput(object data)
    {
        if (data == null) return;
        isBlockInput = (bool)data;
    }

    private void Update()
    {
        InvokeActionKeyDown();
        InvokeActionKey();
        InvokeActionKeyUp();
    }

    private void InvokeActionKey()
    {
        KeyCode key = KeyCode.None;

        if (Input.GetKey(KeyCode.A))
        {
            key = KeyCode.A;
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            key = KeyCode.Mouse0;
        }

        if (lstKeyActions.ContainsKey(key) == false) return;
        List<string> actions = lstKeyActions[key];
        if (actions == null) return;
        int length = actions.Count;
        for (int i = 0; i < length; i++)
        {
            string action_name = actions[i];
            if (isBlockKey(key, action_name)) continue;
            if (dicKeyActions.ContainsKey(action_name))
                dicKeyActions[action_name]?.Invoke();
        }
    }

    private void InvokeActionKeyUp()
    {
        KeyCode key = KeyCode.None;

        if (Input.GetKeyUp(KeyCode.W))
        {
            key = KeyCode.W;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            key = KeyCode.A;
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            key = KeyCode.S;
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            key = KeyCode.D;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            key = KeyCode.Space;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            key = KeyCode.LeftShift;
        }
        if (Input.GetKeyUp(KeyCode.RightShift))
        {
            key = KeyCode.RightShift;
        }
        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            key = KeyCode.LeftAlt;
        }
        if (Input.GetKeyUp(KeyCode.RightAlt))
        {
            key = KeyCode.RightAlt;
        }
        if (Input.GetKeyUp(KeyCode.Return))
        {
            key = KeyCode.Return;
        }
        if (Input.GetKeyUp(KeyCode.F1))
        {
            key = KeyCode.F1;
        }
        if (Input.GetKeyUp(KeyCode.F2))
        {
            key = KeyCode.F2;
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            key = KeyCode.Mouse0;
        }
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            key = KeyCode.Escape;
        }
        if (Input.GetKeyUp(KeyCode.T))
        {
            key = KeyCode.T;
        }
        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            key = KeyCode.LeftAlt;
        }
        if (Input.GetKeyUp(KeyCode.B))
        {
            key = KeyCode.B;
        }
        if (Input.GetKeyUp(KeyCode.P))
        {
            key = KeyCode.P;
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            key = KeyCode.Tab;
        }
        if (Input.GetKeyUp(KeyCode.H))
        {
            key = KeyCode.H;
        }
        if (Input.GetKeyUp(KeyCode.KeypadEnter))
        {
            key = KeyCode.KeypadEnter;
        }
        if (lstKeyUpActions.ContainsKey(key) == false) return;
        List<string> actions = lstKeyUpActions[key];
        if (actions == null) return;
        int length = actions.Count;
        for (int i = 0; i < length; i++)
        {
            string action_name = actions[i];
            if (isBlockKey(key, action_name)) continue;
            if (dicKeyUpActions.ContainsKey(action_name))
                dicKeyUpActions[action_name]?.Invoke();
        }
    }

    private void InvokeActionKeyDown()
    {
        KeyCode key = KeyCode.None;
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            key = KeyCode.LeftShift;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            key = KeyCode.Space;
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            key = KeyCode.F;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            key = KeyCode.LeftShift;
        }
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            key = KeyCode.RightShift;
        }
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            key = KeyCode.LeftAlt;
        }
        if (Input.GetKeyDown(KeyCode.RightAlt))
        {
            key = KeyCode.RightAlt;
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            key = KeyCode.T;
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            key = KeyCode.M;
        }
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            key = KeyCode.LeftAlt;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            key = KeyCode.Escape;
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            key = KeyCode.Mouse0;
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            key = KeyCode.Tab;
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            key = KeyCode.H;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            key = KeyCode.P;
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            key = KeyCode.Q;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            key = KeyCode.E;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            key = KeyCode.R;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            key = KeyCode.Alpha1;
        }
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            key = KeyCode.Keypad1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            key = KeyCode.Alpha2;
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            key = KeyCode.Keypad2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            key = KeyCode.Alpha3;
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            key = KeyCode.Keypad3;
        }

        if (lstKeyDownActions.ContainsKey(key) == false) return;
        List<string> actions = lstKeyDownActions[key];
        if (actions == null) return;
        int length = actions.Count;
        for (int i = 0; i < length; i++)
        {
            string action_name = actions[i];
            if (isBlockKey(key, action_name)) continue;
            if (dicKeyDownActions.ContainsKey(action_name))
                dicKeyDownActions[action_name]?.Invoke();
        }
    }

    private bool isBlockKey(KeyCode key, string action_name)
    {
        if (isBlockInput)
        {
            if (!lstKeyAvailable.ContainsKey(key)) return true;
            List<string> action_names = lstKeyAvailable[key];
            if (action_names != null && action_names.Count > 0 && !action_names.Contains(action_name)) return true;
        }
        return false;
    }
    public void SetlstKeyAvailable(Dictionary<KeyCode, List<string>> lst_key_available)
    {
        lstKeyAvailable.Clear();
        lstKeyAvailable = lst_key_available;
    }
    public void ClearAllEvents()
    {
        isBlockInput = false;
        lstKeyAvailable.Clear();
        lstKeyActions.Clear();
        lstKeyUpActions.Clear();
        lstKeyDownActions.Clear();
        dicKeyActions.Clear();
        dicKeyUpActions.Clear();
        dicKeyDownActions.Clear();
    }
}
