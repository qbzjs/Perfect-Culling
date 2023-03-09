using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractMeetEffect : InteractMissionEffect
{
    public RecordMissionDailyInfo record_missionDaily;
    public override void Init(GameObject ob1, ResponseInteraction ob2, Action on_done)
    {
        onDone = on_done;
        QuestManager.CompleteQuestDaily(record_missionDaily,true);
        Destroy(ob2.GetComponent<ResponseMeetInteractionComponent>());
        OnDone();
    }
    private void SetMission(object data)
    {
        record_missionDaily = (RecordMissionDailyInfo)data;
    }
    private void Awake()
    {
        Observer.Instance.AddObserver(ObserverKey.PushRecordMissionMeetDaily, SetMission);
    }
    private void OnDestroy()
    {
        Observer.Instance.RemoveObserver(ObserverKey.PushRecordMissionMeetDaily, SetMission);
    }
}
