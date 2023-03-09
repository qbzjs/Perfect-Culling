using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunMissionStart : MonoBehaviour
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
            Debug.LogError("dataaa + " + record_mission.mission_name);
            isIn = true;
            PopUpNotice popupNotice = PanelManager.Show<PopUpNotice>();
            popupNotice.OnSetTextTwoButtonCustom("Start Mission", "You have only " + record_mission.time_limit + "s to complete mission. Are you sure to do this?",
                () => {
                    Vector3 position = new Vector3(record_mission.finish_position[0], record_mission.finish_position[1], record_mission.finish_position[2]);
                    DirectionToTarget directionToTarget = GameObject.FindObjectOfType<DirectionToTarget>();
                    if (directionToTarget != null)
                    {
                        directionToTarget.SetTargetPosition(position);
                    }
                    PopupQuest popupQuest = PanelManager.Show<PopupQuest>();
                    popupQuest.transform.SetAsFirstSibling();
                    popupQuest.GetCurrentQuest(QuestManager.numberIDQuestDaily(record_mission.mission_id)).UpdateQuestRepeating(position, record_mission.time_limit, 1);

                    Minimap minimap = PanelManager.Show<Minimap>();
                    minimap.SetMissionTargetPosition(position);

                    GameObject ob = PrefabsManager.Instance.GetAsset<GameObject>("RunMissionTarget");
                    if (ob == null) return;
                    RunMissionTarget runMissionTarget = CreateController.instance.CreateObjectGetComponent<RunMissionTarget>(ob, Vector3.zero);
                    runMissionTarget.record_missionDaily = record_mission;
                    runMissionTarget.transform.parent = transform.parent;
                    runMissionTarget.transform.position = position;
                    GameConfig.gameBlockInput = false;
                    Ultis.SetActiveCursor(false);
                    Destroy(gameObject);
                },()=> {
                    GameConfig.gameBlockInput = false;
                    Ultis.SetActiveCursor(false);
                }, "Start","Cancel");
            GameConfig.gameBlockInput = true;
            Ultis.SetActiveCursor(true);
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
