using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectQuestController : MonoBehaviour
{
    private HighLightNPC previousHighlight;
    void Awake()
    {
        Observer.Instance.AddObserver(ObserverKey.ChangeQuest, OnChangeQuest);
        Observer.Instance.AddObserver(ObserverKey.ChangeQuestDaily, OnChangeQuestDaily);
    }
    private void OnChangeQuest(object data)
    {
        if (previousHighlight !=null &&previousHighlight.gameObject != null)
        {
            Destroy(previousHighlight.gameObject);
        }
        if (data == null) {
            return; 
        }
        RecordMissionInteractionInfo record_mission = (RecordMissionInteractionInfo)data;
        if (record_mission.target_object_name.ToLower().Contains("mia"))
        {
            Vector3 position = new Vector3(record_mission.target_position[0], record_mission.target_position[1], record_mission.target_position[2]);
            GameObject ob = PrefabsManager.Instance.GetAsset<GameObject>("HighlightPlayer");
            HighLightNPC highLightNPC = CreateController.instance.CreateObjectGetComponent<HighLightNPC>(ob, Vector3.zero);
            highLightNPC.transform.position = position;
            previousHighlight = highLightNPC;
        }
    }
    private void OnChangeQuestDaily(object data)
    {
        if (previousHighlight != null && previousHighlight.gameObject != null)
        {
            Destroy(previousHighlight.gameObject);
        }
        if (data == null)
        {
            return;
        }
        RecordMissionDailyInfo record_mission = (RecordMissionDailyInfo)data;
        if (record_mission.target_object_name.ToLower().Contains("goal"))
        {
            Vector3 position = new Vector3(record_mission.target_position[0], record_mission.target_position[1], record_mission.target_position[2]);
            GameObject ob = PrefabsManager.Instance.GetAsset<GameObject>("RunMissionGoal");
            if (ob == null) return;
            RunMissionGoal runMissionGoal = CreateController.instance.CreateObjectGetComponent<RunMissionGoal>(ob, Vector3.zero);
            runMissionGoal.record_missionDaily = record_mission;
            runMissionGoal.transform.parent = transform;
            runMissionGoal.transform.position = position;
        }
        else if (record_mission.mission_type_name.ToLower().Contains("meet"))
        {
            Vector3 position = new Vector3(record_mission.target_position[0], record_mission.target_position[1], record_mission.target_position[2]);
            RecordMissionDailyInfo recordMissionDailyInfo = GetRecordRemain(record_mission);
            Observer.Instance.Notify(ObserverKey.PushRecordMissionMeetDaily, record_mission);
            GameObject ob = PrefabsManager.Instance.GetAsset<GameObject>("HighlightPlayer");
            HighLightNPC highLightNPC = CreateController.instance.CreateObjectGetComponent<HighLightNPC>(ob, Vector3.zero);
            highLightNPC.transform.position = position;
            previousHighlight = highLightNPC;
        }
        else if (record_mission.target_object_name.ToLower().Contains("tivi"))
        {
            Vector3 position = new Vector3(record_mission.target_position[0], record_mission.target_position[1], record_mission.target_position[2]);
            GameObject ob = PrefabsManager.Instance.GetAsset<GameObject>("WatchVideoMission");
            if (ob == null) return;
            WatchVideoMission watchVideoMission = CreateController.instance.CreateObjectGetComponent<WatchVideoMission>(ob, Vector3.zero);
            watchVideoMission.transform.parent = transform;
            watchVideoMission.transform.position = position;
            watchVideoMission.record_mission = record_mission;
        }
        else if (record_mission.mission_type_name.ToLower().Contains("agent"))
        {
            Vector3 position = new Vector3(record_mission.target_position[0], record_mission.target_position[1], record_mission.target_position[2]);
            Observer.Instance.Notify(ObserverKey.PushRecordMissionQuizDaily, record_mission);
            GameObject ob = PrefabsManager.Instance.GetAsset<GameObject>("HighlightPlayer");
            HighLightNPC highLightNPC = CreateController.instance.CreateObjectGetComponent<HighLightNPC>(ob, Vector3.zero);
            highLightNPC.transform.position = position;
            previousHighlight = highLightNPC;
        }
        else if (record_mission.target_object_name.ToLower().Contains("run"))
        {
            Vector3 position = new Vector3(record_mission.target_position[0], record_mission.target_position[1], record_mission.target_position[2]);
            GameObject ob = PrefabsManager.Instance.GetAsset<GameObject>("RunMissionStart");
            if (ob == null) return;
            RunMissionStart runMissionStart = CreateController.instance.CreateObjectGetComponent<RunMissionStart>(ob, Vector3.zero);
            runMissionStart.record_mission = record_mission;
            runMissionStart.transform.parent = transform;
            runMissionStart.transform.position = position;
        }
        
    }
    private RecordMissionDailyInfo GetRecordRemain(RecordMissionDailyInfo record)
    {
        RecordMissionDailyInfo[] recordMissionDailyInfos = QuestManager.getRecordMissionDailyInfoArray;
        if (UserDatas.missionDailyInfo != null&&UserDatas.missionDailyInfo.Length!=0)
        {
            int current_add = 0;
            int length = recordMissionDailyInfos.Length;
            RecordMissionDailyInfo[] recordAdds = new RecordMissionDailyInfo[recordMissionDailyInfos.Length];
            for (int i = 0; i < length; i++)
            {
                if(record.target_object_name == recordMissionDailyInfos[i].target_object_name)
                {
                    recordAdds[current_add] = recordMissionDailyInfos[i];
                    current_add++;
                }
            }
            var recordExcept =  recordAdds.Except(UserDatas.missionDailyInfo);
            if (recordExcept.ToArray<RecordMissionDailyInfo>() != null)
            {
                return recordExcept.ToArray<RecordMissionDailyInfo>()[0];
            }

        }
        return record;

    }
    void OnDestroy()
    {
        Observer.Instance.RemoveObserver(ObserverKey.ChangeQuest, OnChangeQuest);
        Observer.Instance.RemoveObserver(ObserverKey.ChangeQuestDaily, OnChangeQuestDaily);
    }
}
