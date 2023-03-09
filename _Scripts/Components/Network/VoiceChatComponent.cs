using CodeStage.AntiCheat.ObscuredTypes;
using Dissonance;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceChatComponent : MonoBehaviour
{
    private ObscuredBool _isPlayer;
    public void SetUpBehaviour(ObscuredBool isPlayer,ObscuredBool is_visible_ow)
    {
        this._isPlayer = isPlayer;
        if (isPlayer)
        {
            if (is_visible_ow)
            {
                AddVoiceComponent("OpenWorld");
            }
            else
            {
                AddVoiceComponent(UserDatas.lastClassIdJoined);
            }
        }
    }

    private void AddVoiceComponent(string room_name)
    {
        AddVoiceBroadcastTrigger(room_name);
        AddVoiceReceiptTrigger(room_name);
    }
    private void AddVoiceBroadcastTrigger(string room_name)
    {
        voiceBroadcastTrigger = gameObject.AddComponent<VoiceBroadcastTrigger>();
        voiceBroadcastTrigger.ChannelType = CommTriggerTarget.Room;
        voiceBroadcastTrigger.RoomName = room_name;
        voiceBroadcastTrigger.BroadcastPosition = true;
        voiceBroadcastTrigger.Mode = CommActivationMode.PushToTalk;
        voiceBroadcastTrigger.InputName = "Talk";
        VoiceChatManager.Instance.localPlayerVoiceBroadcastTrigger = voiceBroadcastTrigger;
    }
    private void AddVoiceReceiptTrigger(string room_name)
    {
        voiceReceiptTrigger = gameObject.AddComponent<VoiceReceiptTrigger>();
        voiceReceiptTrigger.RoomName = room_name;
        VoiceChatManager.Instance.localVoiceReceiptTrigger = voiceReceiptTrigger;
    }
    private VoiceBroadcastTrigger voiceBroadcastTrigger;
    private VoiceReceiptTrigger voiceReceiptTrigger;

}
