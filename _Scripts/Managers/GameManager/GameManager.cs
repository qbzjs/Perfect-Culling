using CodeStage.AntiCheat.ObscuredTypes;
using Colyseus;
using NativeWebSocket;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using System.Linq;

public class GameManager : MonoBehaviour
{

    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private PlayerManager playerManager;
    [SerializeField]
    Transform playerParent;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(IEInit(delegate
        {
            RegisterOpenworldRoomEvents();
        }));
    }
    private void Awake()
    {
        UserDatas.current_room_type = CurrentRoomType.Openworld;
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.LeftAlt, "ActiveCursor", ActiveCursor, ActionKeyType.Down);
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.LeftAlt, "DeactiveCursor", DeactiveCursor, ActionKeyType.Up);
        Observer.Instance.AddObserver(ObserverKey.TransformPlayer, GetPositionPlayer);
        Observer.Instance.AddObserver(ObserverKey.SetActiveMissionMain, CreateQuest);
        Observer.Instance.AddObserver(ObserverKey.SetActiveMissioDaily, CreateQuestDaily);

    }
    private void GetPositionPlayer(object data)
    {
        Vector3 localPosition = playerParent.GetChild(0).transform.position;
        Observer.Instance.Notify(ObserverKey.DistanceMisison, localPosition);
    }
    private void OnDestroy()
    {
        StopAllCoroutines();
        StopAllCoroutines();
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.LeftAlt, "ActiveCursor", ActiveCursor, ActionKeyType.Down);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.LeftAlt, "DeactiveCursor", DeactiveCursor, ActionKeyType.Up);
        Observer.Instance.RemoveObserver(ObserverKey.MapTeleport, TeleportPlayer);
        Observer.Instance.RemoveObserver(ObserverKey.TransformPlayer, GetPositionPlayer);
        Observer.Instance.RemoveObserver(ObserverKey.SetActiveMissionMain, CreateQuest);
        Observer.Instance.RemoveObserver(ObserverKey.SetActiveMissioDaily, CreateQuestDaily);
    }

    private IEnumerator IEInit(UnityAction action)
    {
        yield return new WaitUntil(() => CanvasManager.instance != null);
        if (mainCamera != null)
        {
            var cam = mainCamera.GetUniversalAdditionalCameraData();
            cam.cameraStack.Add(CanvasManager.instance.canvasCamera);
        }
        yield return new WaitUntil(() => NetworkingManager.Instance.openworldRoom != null);
        action?.Invoke();
    }

    private void RegisterOpenworldRoomEvents()
    {
        if (playerManager != null)
            playerManager.RegisterOpenworldRoomEvents(CreateAllQuest);
        
    }

    private void ResetTransform(Transform new_transform)
    {
        PopupQuest popupQuest = PanelManager.Show<PopupQuest>();
        if (popupQuest != null)
        {
            popupQuest.SetPlayerTransform(new_transform);
        }
        Minimap minimap = PanelManager.Show<Minimap>();
        if (minimap != null)
        {
            minimap.SetPlayerTransform(new_transform);
        }
        int current_quest = UserDatas.user_Data.info.current_id_main_mission ;
        if (current_quest == -1)
        {
            if (QuestManager.MissionEntityRemain.Length != 0)
            {
                RecordMissionDailyInfo record_mission = QuestManager.getRecordMissionDailyInfo(0);
                Vector3 targetPosition = new Vector3(record_mission.target_position[0], record_mission.target_position[1], record_mission.target_position[2]);
                CreateDirectionToTargetObject(targetPosition, new_transform);
            }
        }
        else
        {
            RecordMissionInteractionInfo record_mission = QuestManager.getRecordMissionInteractionInfo(current_quest);
            Vector3 targetPosition = new Vector3(record_mission.target_position[0], record_mission.target_position[1], record_mission.target_position[2]);
            CreateDirectionToTargetObject(targetPosition, new_transform);
        }

    }
    private void CreateAllQuest()
    {
        Observer.Instance.AddObserver(ObserverKey.MapTeleport, TeleportPlayer);
        if (UserDatas.user_Data.info.is_tutorial_done && UserDatas.user_Data.info.current_id_main_mission !=-1)
        {
            CreateQuest();
       
        }
        if (UserDatas.user_Data.info.open_mission_daily)
        {
            CreateQuestDaily();
        }
        ResetTransform(playerParent.GetChild(0).transform);
    }
    private void CreateQuest(object data=null)
    {
        int current_quest = UserDatas.user_Data.info.current_id_main_mission;
        RecordMissionInteractionInfo record_mission = QuestManager.getRecordMissionInteractionInfo(current_quest);
        RecordInteractionTypeObserver interaction_type = new RecordInteractionTypeObserver();
        interaction_type.interaction_type = ResponseInteractionType.Quest;
        interaction_type.mission_id = record_mission.mission_id;
        interaction_type.object_name = record_mission.target_object_name;
        interaction_type.mission_name = record_mission.mission_name;
        interaction_type.mission_type_name = record_mission.mission_type_name;
        interaction_type.mission_type_quest = record_mission.mission_type_quest;
        interaction_type.mission_type = record_mission.mission_type;
        Vector3 targetPosition = new Vector3(record_mission.target_position[0], record_mission.target_position[1], record_mission.target_position[2]);
        OpenPopupQuest(interaction_type, targetPosition);
        OpenMinimap(targetPosition, record_mission.target_object_name);
        Observer.Instance.Notify(ObserverKey.UpdateResponseInteractionInfo, interaction_type);
        Observer.Instance.Notify(ObserverKey.ChangeQuest, record_mission);
        
    }
    private void CreateQuestDaily(object data = null)
    {
        RecordMissionDailyInfo[] recordMissionDailyInfos = QuestManager.getRecordMissionDailyInfoArray;
        RecordInteractionTypeObserver interaction_type = new RecordInteractionTypeObserver();
        interaction_type.interaction_type = ResponseInteractionType.Quest;
        foreach (var item in recordMissionDailyInfos)
        {
            foreach (var itemRemain in QuestManager.MissionEntityRemain)
            {
                if (itemRemain.mission_id == item.mission_id)
                {
                    interaction_type.mission_id = item.mission_id;
                    interaction_type.object_name = item.target_object_name;
                    interaction_type.mission_name = item.mission_name;
                    interaction_type.mission_type_name = item.mission_type_name;
                    interaction_type.mission_type_quest = item.mission_type_quest;
                    interaction_type.mission_type = item.mission_type;
                    Vector3 targetPosition = new Vector3(item.target_position[0], item.target_position[1], item.target_position[2]);
                    OpenPopupQuest(interaction_type, targetPosition);
                    Observer.Instance.Notify(ObserverKey.UpdateResponseInteractionInfo, interaction_type);
                    Observer.Instance.Notify(ObserverKey.ChangeQuestDaily, item);
                    break;
                }
            }
        }
        Minimap minimap = PanelManager.Show<Minimap>();
        if (minimap != null)
        {
            minimap.SetPlayerTransform(playerManager.playerMe.transform);
        }
    }
    
    private void CreateDirectionToTargetObject(Vector3 target, Transform new_transform)
    {
        GameObject ob = PrefabsManager.Instance.GetAsset<GameObject>("DirectionToTargetObject");
        if (ob == null) return;
        DirectionToTarget directionToTarget = CreateController.instance.CreateObjectGetComponent<DirectionToTarget>(ob, Vector3.zero, new_transform, "");
        if (directionToTarget != null)
        {
            directionToTarget.transform.parent = new_transform;
            directionToTarget.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
            directionToTarget.transform.localScale = 0.4f * Vector3.one;
            directionToTarget.SetTargetPosition(target);
        }
    }
    private void OpenPopupQuest(RecordInteractionTypeObserver interaction_type, Vector3 target_position)
    {
        PopupQuest popupQuest = PanelManager.Show<PopupQuest>();
        if (popupQuest != null)
        {
            popupQuest.transform.SetAsFirstSibling();
            popupQuest.SetPlayerTransform(playerManager.playerMe.transform);
            int idQuest = 0;
            if (interaction_type.mission_type_quest == "main")
            {
                idQuest = interaction_type.mission_id;
            }
            else if(interaction_type.mission_type_quest == "daily")
            {
                idQuest = QuestManager.numberIDQuestDaily(interaction_type.mission_id);
            }
            popupQuest.AddQuestWithParameter(idQuest, interaction_type.mission_name, target_position, interaction_type.mission_type_quest, interaction_type.mission_type_name);
        }
    }
    private void OpenMinimap(Vector3 mission_target_position, string mission_target_name)
    {
        Minimap minimap = PanelManager.Show<Minimap>();
        if (minimap != null)
        {
            minimap.SetPlayerTransform(playerManager.playerMe.transform);
            minimap.SetMissionTargetPosition(mission_target_position);
        }
    }
    private void TeleportPlayer(object data)
    {
        string place = (string)data;
        Position position = DataController.Instance.TeleportVO.GetDataByName<Position>("TeleportInfo", place);
        Transform player = playerManager.playerMe.transform;
        CharacterController characterController = player.GetComponent<CharacterController>();
        characterController.enabled = false;
        player.position = new Vector3(position.x, position.y, position.z);
        characterController.enabled = true;
    }
    private void ActiveCursor()
    {
        Ultis.SetActiveCursor(true);
    }
    private void DeactiveCursor()
    {
        Ultis.SetActiveCursor(false);
    }

    private void OnApplicationQuit()
    {
        StopAllCoroutines();
        ImageManager.instance.DestroyTexturesUnuse(true);
    }
}
