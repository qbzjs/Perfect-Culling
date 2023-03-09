using CodeStage.AntiCheat.ObscuredTypes;
using Dissonance;
using Dissonance.Audio.Playback;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceChatManager : TPRLSingleton<VoiceChatManager>
{
    private ObscuredBool _isMicroPhoneFound = false;
    public ObscuredBool isMicroPhoneFound => _isMicroPhoneFound;
    private Coroutine corRequestPermission = null;
    [SerializeField]
    private PopUpNotice popUpNotice;

    private void Start()
    {
        IsPermissionRequested();
    }

    protected override void OnDestroy()
    {
        StopAllCoroutines();
        base.OnDestroy();
    }

    public bool IsPermissionRequested()
    {
        if(isMicroPhoneFound == false)
        {
            RequestPermission();
            return false;
        }
        return true;
    }

    private void RequestPermission()
    {
        if (corRequestPermission != null)
            StopCoroutine(corRequestPermission);
        corRequestPermission = StartCoroutine(IERequestPermission());
    }

    private IEnumerator IERequestPermission()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
        if (Application.HasUserAuthorization(UserAuthorization.Microphone))
        {
            _isMicroPhoneFound = true;
            Debug.LogError("Microphone found");
        }
        else
        {
            _isMicroPhoneFound = false;
            Debug.LogError("Microphone not found");
            if (popUpNotice != null)
                popUpNotice.OnSetTextTwoButtonCustom(Constant.NOTICE, "Microphone not found", delegate { IsPermissionRequested(); }, null, "Retry", "Ignore");
        }


    }

    void FindMicrophones()
    {
        foreach (var device in Microphone.devices)
        {
            Debug.LogError("Microphone name: " + device);
        }
    }


    private DissonanceComms _dissonanceComms;
    private DissonanceComms dissonanceComms
    {
        get
        {
            if (_dissonanceComms == null) _dissonanceComms = GetComponent<DissonanceComms>();
            return _dissonanceComms;
        }
    }
    private  AudioSource voicePlaybackAudioSource
    {
        get
        {
            return dissonanceComms.PlaybackPrefab.GetComponent<AudioSource>();
        }
    }
    [HideInInspector]
    public VoiceBroadcastTrigger localPlayerVoiceBroadcastTrigger;
    [HideInInspector]
    public VoiceReceiptTrigger localVoiceReceiptTrigger;
    [HideInInspector]
    public AudioListener localAudioListener;
    public void SetMuteVoice(bool is_mute)
    {
        dissonanceComms.IsMuted = is_mute;
    }
    public void ChangeRoom(string room_name)
    {
        localPlayerVoiceBroadcastTrigger.ChannelType = CommTriggerTarget.Room;
        localPlayerVoiceBroadcastTrigger.RoomName = room_name;
        localVoiceReceiptTrigger.RoomName = room_name;
    }
    public void ChangeMode(CommActivationMode mode)
    {
        localPlayerVoiceBroadcastTrigger.Mode = mode;
    }
    public CommActivationMode currentVoiceMode
    {
        get
        {
            return localPlayerVoiceBroadcastTrigger.Mode;
        }
    }
}
