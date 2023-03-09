using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonVoice : MonoBehaviour
{
    private Button buttonVoice;
    private Image buttonImage;
    [SerializeField] private List<Sprite> icon;
    private bool isMute = false;
    private bool isTalking =false;
    private void Awake()
    {
        buttonVoice = GetComponent<Button>();
        buttonVoice.onClick.AddListener(SetMute);
        buttonImage = GetComponent<Image>();
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.H, "Hold", Hold, ActionKeyType.Down);
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.H, "Release", Release, ActionKeyType.Up);
    }

    private void OnDestroy()
    {
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.H, "Hold", Hold, ActionKeyType.Down);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.H, "Release", Release, ActionKeyType.Up);
    }

    private void Update()
    {
        if(VoiceChatManager.Instance!=null && VoiceChatManager.Instance.localPlayerVoiceBroadcastTrigger != null)
        {
            if (VoiceChatManager.Instance.localPlayerVoiceBroadcastTrigger.IsTransmitting)
            {
                Talking();
            }
            else
            {
                NotTalking();
            }
        }
    }
    private void Hold()
    {
        if(VoiceChatManager.Instance.currentVoiceMode == Dissonance.CommActivationMode.PushToTalk)
        {
            Talking();
        }
    }

    private void Release()
    {
        if (VoiceChatManager.Instance.currentVoiceMode == Dissonance.CommActivationMode.PushToTalk)
        {
            NotTalking();
        }
    }
    private void Talking()
    {
        isTalking = true;
        ChangeSprite();
    }
    private void NotTalking()
    {
        isTalking = false;
        ChangeSprite();
    }

    private void ChangeSprite()
    {
        if (isMute)
        {
            if (isTalking)
            {
                buttonImage.sprite = icon[3];
            }
            else
            {
                buttonImage.sprite = icon[2];
            }
        }
        else
        {
            if (isTalking)
            {
                buttonImage.sprite = icon[1];
            }
            else
            {
                buttonImage.sprite = icon[0];
            }
        }
    }
    private void SetMute()
    {
        isMute = !isMute;
        VoiceChatManager.Instance.SetMuteVoice(isMute);
        ChangeSprite();
    }
}
