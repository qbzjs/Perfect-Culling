using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatchVideoMission : MonoBehaviour
{
    public RecordMissionDailyInfo record_mission;
    [SerializeField] private ParticleSystem effects;
    private bool isIn = false;
    private void Awake()
    {
        effects.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isIn && other.gameObject.layer == LayerMask.NameToLayer("LayerChat"))
        {
            isIn = true;
            Vector3 position = new Vector3(record_mission.finish_position[0], record_mission.finish_position[1], record_mission.finish_position[2]);
            DirectionToTarget directionToTarget = GameObject.FindObjectOfType<DirectionToTarget>();
            if (directionToTarget != null)
            {
                directionToTarget.SetTargetPosition(position);
            }
            PopupQuest popupQuest = PanelManager.Show<PopupQuest>();
            popupQuest.transform.SetAsFirstSibling();
            popupQuest.GetCurrentQuest(QuestManager.numberIDQuestDaily(record_mission.mission_id)).targetPosition = position;

            Minimap minimap = PanelManager.Show<Minimap>();
            minimap.SetMissionTargetPosition(position);
            InteractWatchVideoMissionEffect.record_MissionDaily = record_mission;
            Destroy(gameObject);
        }
        else
        {
            return;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (isIn && other.gameObject.layer == LayerMask.NameToLayer("LayerChat"))
        {
            isIn = false;
        }
    }
}
