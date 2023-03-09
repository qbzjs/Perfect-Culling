using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractSitMissionEffect : InteractMissionEffect
{
    public override void Init(GameObject ob1, ResponseInteraction ob2, Action on_done)
    {
        onDone = on_done;
        record_mission = QuestManager.getRecordMissionInteractionInfo(UserDatas.user_Data.info.current_id_main_mission);

        Dictionary<KeyCode, List<string>> listAvailableKey = new Dictionary<KeyCode, List<string>>();
        listAvailableKey.Add(KeyCode.LeftShift, null);
        listAvailableKey.Add(KeyCode.RightShift, null);
        InputRegisterEvent.Instance.SetlstKeyAvailable(listAvailableKey);

        SitComponent sitComponent = ob1.AddComponent<SitComponent>();
        EntityManager entityManager = ob1.GetComponent<EntityManager>();
        entityManager.sit = sitComponent;
        sitComponent.OnUnsitCallback = ()=> { CompleteQuest(sitComponent); };

        Vector3 targetPosition = new Vector3(record_mission.target_position[0], record_mission.target_position[1], record_mission.target_position[2]);
        sitComponent.Sit(targetPosition);
        sitComponent.SitRotation(new Vector3(0, 180, 0));
    }

    private RecordMissionInteractionInfo record_mission;
    
    private void CompleteQuest(SitComponent sit_component)
    {
        RecordInteractionTypeObserver interaction_type = QuestManager.GetInteractionTypeObserver(ResponseInteractionType.None, -1, record_mission.target_object_name, "","","","");
        Observer.Instance.Notify(ObserverKey.UpdateResponseInteractionInfo, interaction_type);
        
        Destroy(sit_component);
        OnDone();
        QuestManager.CompleteQuest(record_mission, true);
    }
}
