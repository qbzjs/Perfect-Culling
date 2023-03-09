using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractTalkMissionEffect : InteractMissionEffect
{
    public override void Init(GameObject ob1, ResponseInteraction ob2, Action on_done)
    {
        onDone = on_done;
        RecordMissionInteractionInfo record_mission = QuestManager.getRecordMissionInteractionInfo(UserDatas.user_Data.info.current_id_main_mission);
        Dictionary<KeyCode, List<string>> listAvailableKey = new Dictionary<KeyCode, List<string>>();
        listAvailableKey.Add(KeyCode.Space, null);
        InputRegisterEvent.Instance.SetlstKeyAvailable(listAvailableKey);

        OpenPopupConversation(ob1, record_mission);

    }
    private void OpenPopupConversation(GameObject ob1, RecordMissionInteractionInfo record_mission)
    {
        GameConfig.gameBlockInput = true;
        Ultis.SetActiveCursor(false);
        PopupConversation popupConversation = PanelManager.Show<PopupConversation>();
        if (popupConversation != null)
        {
            popupConversation.OnCloseCallback = delegate
            {
                RecordInteractionTypeObserver interaction_type = QuestManager.GetInteractionTypeObserver(ResponseInteractionType.None, -1, record_mission.target_object_name, "", "", "", "");
                Observer.Instance.Notify(ObserverKey.UpdateResponseInteractionInfo, interaction_type);
                OnDone();
                QuestManager.CompleteQuest(record_mission,true);
            };
            popupConversation.InitPopUpConversation(record_mission.target_object_name, record_mission.conversation_string);
        }
    }
   
}
