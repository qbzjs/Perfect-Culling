using System.Collections.Generic;
using System.Linq;
using System.Net;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public static class QuestManager
{
    private static int _numberIDQuestDaily = 100;
    public static int numberIDQuestDaily(int questDaily)
    {
        return _numberIDQuestDaily + questDaily;
    }
    private static string _currentObjectName;
    public static string currentObjectName
    {
        get
        {
            //return ObscuredPrefs.Get(PlayerPrefsString.CurrentQuestObjectName, "");
            return _currentObjectName;
            //return "";
        }
        set
        {
            _currentObjectName = value;
            //ObscuredPrefs.Set(PlayerPrefsString.CurrentQuestObjectName, value);
        }
    }
    private static int _current_mission_daily = 0;
    public static int current_mission_daily
    {
        get
        {
            return _current_mission_daily;
        }
        set
        {
            _current_mission_daily = value;
        }
    }
    private static RecordMissionInteractionInfo _recordMissionInteractionInfo;
    public static RecordMissionInteractionInfo getRecordMissionInteractionInfo(int current_mission,bool call=true)
    {
        if (current_mission < 0 ) return _recordMissionInteractionInfo;
        RecordMissionInteractionInfo[] recordMissionInteractionInfos = DataController.Instance.Mission_VO.GetDatasByName<RecordMissionInteractionInfo>("MissionInfo");
        if (recordMissionInteractionInfos == null || current_mission >= recordMissionInteractionInfos.Length) return default;
        if(call) UserDatas.user_Data.info.current_id_main_mission = current_mission;
        _recordMissionInteractionInfo = recordMissionInteractionInfos[current_mission];
        return _recordMissionInteractionInfo;
    }

    public static int numberMissionWelcomeEnd
    {
        get
        {
            int numberEnd = 0;
            RecordMissionInteractionInfo[] recordMissionInteractionInfos = DataController.Instance.Mission_VO.GetDatasByName<RecordMissionInteractionInfo>("MissionInfo");
            int length = recordMissionInteractionInfos.Length;
            for (int i = 0; i < length; i++)
            {
                var record_mission = recordMissionInteractionInfos[i];
                if (!record_mission.mission_type_name.ToLower().Contains("welcome"))
                {
                    break;
                }
                else
                {
                    numberEnd++;
                }
            }
            return numberEnd;
        }

    } 
    private static RecordMissionDailyInfo  _recordMissionDailyInfo;
    public static RecordMissionDailyInfo  getRecordMissionDailyInfo(int current_mission)
    {
      if (current_mission< 0 ) return _recordMissionDailyInfo;
        RecordMissionDailyInfo [] recordMissionDailyInfos = DataController.Instance.Mission_VO.GetDatasByName<RecordMissionDailyInfo>("DailyMissionInfo");
        if (recordMissionDailyInfos == null || current_mission >= recordMissionDailyInfos.Length) return default;
        _recordMissionDailyInfo = recordMissionDailyInfos[current_mission];
        return _recordMissionDailyInfo;
    }
    private static RecordMissionDailyInfo[] _recordMissionDailyInfoArray;
    public static RecordMissionDailyInfo [] getRecordMissionDailyInfoArray
    {
        get
        {
            RecordMissionDailyInfo[] recordMissionDailyInfos = DataController.Instance.Mission_VO.GetDatasByName<RecordMissionDailyInfo>("DailyMissionInfo");
            _recordMissionDailyInfoArray = recordMissionDailyInfos ;
            return _recordMissionDailyInfoArray;
        }
       
    }
    public static RecordReward _recordReward;
    public static RecordReward getRecordRewardByKey(string type, int id)
    {
        RecordReward[] recordRewards = DataController.Instance.Mission_VO.GetDatasByName<RecordReward>("ItemInfo");
        if (recordRewards == null || id >= recordRewards.Length) return default;
        foreach (var record in recordRewards)
        {
            if (record.id == id&&record.type==type)
            {
                _recordReward = record;
                return _recordReward;
            }
        };
        return recordRewards[0];
    }
    public static RecordMissionDailyInfo[] MissionEntityRemain
    {
        get
        {
            var record = UserDatas.missionDailyInfo.Except(UserDatas.missionDailyInfoUsed);
            return record.ToArray<RecordMissionDailyInfo>();
        }
    }
    public static void CompleteQuestDaily(RecordMissionDailyInfo record_missionDaily,bool is_complete)
    {
        string token = UserDatas.token;
        string address = UserDatas.user_Data.info.address;
        TPRLAPI.instance.PostUpdateProgressResponse(token, address, "daily", record_missionDaily.mission_id, 1, (data) =>
        {
            Debug.LogError(data);
            RecordUserUpdateProgressReponse myDeserializedClass = JsonUtility.FromJson<RecordUserUpdateProgressReponse>(data);
            if (myDeserializedClass.output.is_completed)
            {
                PopupQuest currentPopupQuest = PanelManager.Show<PopupQuest>();
                if (currentPopupQuest != null)
                {
                    currentPopupQuest.CompleteQuest(numberIDQuestDaily(record_missionDaily.mission_id));
                }
                UserDatas.missionDailyInfoUsed[_current_mission_daily] = record_missionDaily;
                TPRLAPI.instance.PostRewardResponse(token, address, "daily", record_missionDaily.mission_id, (dataReward) =>
                {
                    RecordRewardResponse recordRewardResponse = JsonUtility.FromJson<RecordRewardResponse>(dataReward);
                    if (recordRewardResponse.output != null && recordRewardResponse.output.Length != 0)
                    {
                        int lengthID = recordRewardResponse.output.Length;
                        RecordReward[] rewards = new RecordReward[lengthID];
                        for (int i = 0; i < lengthID; i++)
                        {
                            RecordReward record = getRecordRewardByKey(recordRewardResponse.output[i].item_type, recordRewardResponse.output[i].item_id);
                            rewards[0].name = record.name;
                            rewards[0].icon = record.icon;
                            rewards[0].amount = recordRewardResponse.output[i].quantity;
                        }
                        PopUpReward popupReward = PanelManager.Show<PopUpReward>();
                        popupReward.SetContent("Reward", rewards);
                        popupReward.actionOk = () => {
                            PanelManager.Hide<PopUpReward>();
                            GameConfig.gameBlockInput = false;
                            Ultis.SetActiveCursor(false);
                            PopUpCompleteQuestEffect popUpCompleteQuestEffect = PanelManager.Show<PopUpCompleteQuestEffect>();
                            popUpCompleteQuestEffect.PlayAnimComplete(true);
                        };
                    }
                    else
                    {
                        PopUpCompleteQuestEffect popUpCompleteQuestEffect = PanelManager.Show<PopUpCompleteQuestEffect>();
                        popUpCompleteQuestEffect.PlayAnimComplete(is_complete);
                        GameConfig.gameBlockInput = false;
                        Ultis.SetActiveCursor(false);
                    }

                });
                RecordMissionInteractionInfo recordMII = getRecordMissionInteractionInfo(UserDatas.user_Data.info.current_id_main_mission);
                Vector3 targetNext = new Vector3(recordMII.target_position[0], recordMII.target_position[1], recordMII.target_position[2]);
                ChangeDirectionTarget(targetNext);
            }

        });

    }
    public static void CompleteQuest(RecordMissionInteractionInfo record_mission, bool is_complete)
    {
        string token = UserDatas.token;
        string address = UserDatas.user_Data.info.address;
        PopupQuest currentPopupQuest = PanelManager.Show<PopupQuest>();
        if (currentPopupQuest != null)
        {
            currentPopupQuest.CompleteQuest(UserDatas.user_Data.info.current_id_main_mission);
        }
        TPRLAPI.instance.PostUpdateProgressResponse(token, address, "main", record_mission.mission_id, 1, (data) =>
        {
            RecordUserUpdateProgressReponse record = JsonUtility.FromJson<RecordUserUpdateProgressReponse>(data);
            if (record.output.is_completed)
            {
                TPRLAPI.instance.PostRewardResponse(token, address, "main", record_mission.mission_id, (dataReward) =>
                {
                    Debug.LogError(dataReward);
                    RecordRewardResponse recordRewardResponse = JsonUtility.FromJson<RecordRewardResponse>(dataReward);
                    if (recordRewardResponse.output != null&&recordRewardResponse.output.Length!=0)
                    {
                        int lengthID = recordRewardResponse.output.Length;
                        RecordReward[] rewards = new RecordReward[lengthID];
                        for (int i = 0; i < lengthID; i++)
                        {
                            RecordReward record = getRecordRewardByKey(recordRewardResponse.output[i].item_type, recordRewardResponse.output[i].item_id);
                            rewards[0].name = record.name;
                            rewards[0].icon = record.icon;
                            rewards[0].amount = recordRewardResponse.output[i].quantity;
                        }
                        PopUpReward popupReward = PanelManager.Show<PopUpReward>();
                        popupReward.SetContent("Reward", rewards);
                        popupReward.actionOk = () => {
                            PanelManager.Hide<PopUpReward>();
                            GameConfig.gameBlockInput = false;
                            Ultis.SetActiveCursor(false);
                            PopUpCompleteQuestEffect popUpCompleteQuestEffect = PanelManager.Show<PopUpCompleteQuestEffect>();
                            popUpCompleteQuestEffect.PlayAnimComplete(true);
                        };
                    }
                    else
                    {
                        PopUpCompleteQuestEffect popUpCompleteQuestEffect = PanelManager.Show<PopUpCompleteQuestEffect>();
                        popUpCompleteQuestEffect.PlayAnimComplete(is_complete);
                        GameConfig.gameBlockInput = false;
                        Ultis.SetActiveCursor(false);
                    }
                });
            }
            InitMission(record_mission.next_mission_id);
        });
        //if (is_complete && !string.IsNullOrEmpty(record_mission.reward.type))
        //{
            
        //    GameConfig.gameBlockInput = true;
        //    Ultis.SetActiveCursor(true);
        //    RecordReward[] rewards = new RecordReward[1];
        //    rewards[0] = new RecordReward();
        //    rewards[0].name = record_mission.reward.name;
        //    rewards[0].icon = record_mission.reward.name;
        //    rewards[0].amount = 1;

        //    PopUpReward popupReward = PanelManager.Show<PopUpReward>();
        //    popupReward.SetContent("Reward", rewards);
        //    popupReward.actionOk = () => {
        //        PanelManager.Hide<PopUpReward>();
        //        GameConfig.gameBlockInput = false;
        //        Ultis.SetActiveCursor(false);
        //        UserDatas.AddCharacter(record_mission.reward.id);
        //        PopUpCompleteQuestEffect popUpCompleteQuestEffect = PanelManager.Show<PopUpCompleteQuestEffect>();
        //        popUpCompleteQuestEffect.PlayAnimComplete(true);
        //    };
        //}
        //else
        //{
        //    PopUpCompleteQuestEffect popUpCompleteQuestEffect = PanelManager.Show<PopUpCompleteQuestEffect>();
        //    popUpCompleteQuestEffect.PlayAnimComplete(is_complete);
        //    GameConfig.gameBlockInput = false;
        //    Ultis.SetActiveCursor(false);
        //}
    }
    public static void InitMission(int mission_id)
    {
        if(mission_id >= QuestManager.numberMissionWelcomeEnd||mission_id==0)
        {
            Observer.Instance.Notify(ObserverKey.SetActiveMissioDaily);
            UserDatas.user_Data.info.open_mission_daily = true;
        }
        if (mission_id>0)
        {
            RecordMissionInteractionInfo record_mission = getRecordMissionInteractionInfo(mission_id);
            UserDatas.user_Data.info.current_id_main_mission = mission_id;
            RecordInteractionTypeObserver interaction_type = GetInteractionTypeObserver(ResponseInteractionType.Quest, record_mission.mission_id, record_mission.target_object_name, record_mission.mission_name, record_mission.mission_type,record_mission.mission_type_quest,record_mission.mission_type_name);
            Vector3 targetPosition = new Vector3(record_mission.target_position[0], record_mission.target_position[1], record_mission.target_position[2]);
            OpenPopupQuest(interaction_type, targetPosition);
            ChangeDirectionTarget(targetPosition);
            Observer.Instance.Notify(ObserverKey.UpdateResponseInteractionInfo, interaction_type);
            Observer.Instance.Notify(ObserverKey.ChangeQuest, record_mission);
        }
        else
        {
            Observer.Instance.Notify(ObserverKey.ChangeQuest, null);
        }
    }

    private static bool OpenPopupQuest(RecordInteractionTypeObserver interaction_type, Vector3 target_position)
    {
        PopupQuest currentPopupQuest = PanelManager.Show<PopupQuest>();
        if (currentPopupQuest != null)
        {
            currentPopupQuest.transform.SetAsFirstSibling();
            return currentPopupQuest.AddQuestWithParameter(interaction_type.mission_id, interaction_type.mission_name, target_position,interaction_type.mission_type_quest,interaction_type.mission_type_name);
        }
        return false;
    }
    private static void ClosePopupQuest()
    {
        PanelManager.Hide<PopupQuest>();
    }
    private static void CloseDirectionToQuestObject()
    {
        DirectionToTarget directionToTarget = GameObject.FindObjectOfType<DirectionToTarget>();
        GameObject.Destroy(directionToTarget.gameObject);
    }
    public static void ChangeDirectionTarget(Vector3 target)
    {
        DirectionToTarget directionToTarget = GameObject.FindObjectOfType<DirectionToTarget>();
        if (directionToTarget != null)
        {
            directionToTarget.SetTargetPosition(target);
        }
    }
    public static  RecordInteractionTypeObserver GetInteractionTypeObserver(ResponseInteractionType type, int mission_id, string target_object_name, string mission_name, string mission_type,string typeQuest,string typeName)
    {
        RecordInteractionTypeObserver interaction_type = new RecordInteractionTypeObserver();
        interaction_type.interaction_type = type;
        interaction_type.mission_id = mission_id;
        interaction_type.object_name = target_object_name;
        interaction_type.mission_name = mission_name;
        interaction_type.mission_type = mission_type;
        interaction_type.mission_type_name = typeName;
        interaction_type.mission_type_quest = typeQuest;
        return interaction_type;
    }
}
