
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;
using System.Collections;

[UIPanelPrefabAttr("PopupConversation", "PopupCanvas")]
public class PopupConversation : BasePanel
{
    [SerializeField] private TMP_Text nPCNameText;
    [SerializeField] private TMP_Text conversationText;
    [SerializeField] private TMP_Text nextText;

    public Action OnCloseCallback;

    protected override void Awake()
    {
        base.Awake();
    }
    private void OnEnable()
    {
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.Space, "GetNextLine", GetNextLine, ActionKeyType.Down);
    }
    private void OnDisable()
    {
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.Space, "GetNextLine", GetNextLine, ActionKeyType.Down);
    }
    private void OnDestroy()
    {
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.Space, "GetNextLine", GetNextLine, ActionKeyType.Down);
    }
    private int currentConversationIndex;

    private string[] conversationString;

    public void InitPopUpConversation(string npc_name, string[] conversation_string)
    {
        nPCNameText.text = npc_name;
        conversationString = conversation_string;
        InitFirstString();
    }
    private void InitFirstString()
    {
        currentConversationIndex = 0;
        SetConversationText(conversationString[0]);
    }
    private void SetConversationText(string conversation_text)
    {
        conversationText.text = conversation_text; 
        StartCoroutine(TextVisible());
    }
    private void SetNextText(string text)
    {
        nextText.text = text;
    }
    private void GetNextLine()
    {
        if (gameObject.activeInHierarchy)
        {
            if (!isTextRunning) {
                currentConversationIndex++;
                if (currentConversationIndex == conversationString.Length - 1)
                {
                    SetNextText(Constant.Finish);
                }
                if (currentConversationIndex == conversationString.Length)
                {
                    SetNextText(Constant.Next);
                    currentConversationIndex = 0;
                    Close();
                }
                else
                {
                    SetConversationText(conversationString[currentConversationIndex]);
                    StartCoroutine(TextVisible());
                }
            }
            else
            {
                conversationText.maxVisibleCharacters = conversationText.textInfo.characterCount;
                isTextRunning = false;
            }
        }
    }
    [SerializeField] float timeBetweenChars;
    [SerializeField] float timeBetweenWords;
    private bool isTextRunning;
    private IEnumerator TextVisible()
    {
        isTextRunning = true;
        conversationText.ForceMeshUpdate();
        int totalVisibleCharacters = conversationText.textInfo.characterCount;
        int counter = 0;
        while (isTextRunning)
        {
            int visibleCount = counter % (totalVisibleCharacters + 1);
            conversationText.maxVisibleCharacters = visibleCount;
            if(visibleCount >= totalVisibleCharacters)
            {
                isTextRunning = false;
                break;
            }
            counter += 1;
            yield return new WaitForSeconds(timeBetweenChars);
        }
    }
    private void Close()
    {
        OnCloseCallback?.Invoke();
        PanelManager.Hide<PopupConversation>();
    }
}
