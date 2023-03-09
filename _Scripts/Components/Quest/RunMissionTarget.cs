using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RunMissionTarget : MonoBehaviour
{
    public RecordMissionDailyInfo record_missionDaily;
    [SerializeField] private ParticleSystem effects;
    private void OnTriggerEnter(Collider other)
    {
        //TODO : With multiplayer check if is player .
        PopupQuest popupQuest = PanelManager.Show<PopupQuest>();
        popupQuest.transform.SetAsFirstSibling();
        popupQuest.GetCurrentQuest(QuestManager.numberIDQuestDaily(record_missionDaily.mission_id)).StopUpdateQuestRepeating();
        //QuestManager.CompleteQuest(true);
        QuestManager.CompleteQuestDaily(record_missionDaily,true);
        //RecordMissionInteractionInfo record_mission = QuestManager.getRecordMissionInteractionInfo(QuestManager.currentQuestID);
        //QuestManager.InitMission(record_mission.next_mission_id);
        Destroy(gameObject);
    }
    private void Awake()
    {
        InvokeRepeating("CheckTime",0,1);
        effects.Play();
    }
    private void CheckTime()
    {
        PopupQuest popupQuest = PanelManager.Show<PopupQuest>();
        popupQuest.transform.SetAsFirstSibling();
        if (popupQuest.GetCurrentQuest(QuestManager.numberIDQuestDaily(record_missionDaily.mission_id)).timeRemaining <= 0)
        {
            Destroy(gameObject);
        }
    }
    private void OnDestroy()
    {
        CancelInvoke();
    }
}