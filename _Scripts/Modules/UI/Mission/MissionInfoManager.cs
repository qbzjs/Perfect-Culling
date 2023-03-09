using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.Storage;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[UIPanelPrefabAttr("MissionInfo", "PopupCanvas")]
public class MissionInfoManager : BasePanel
{
    [SerializeField] Button imageMain;
    [SerializeField] Button imageDaily;
    [SerializeField] Image imageArrowMain;
    [SerializeField] Image imageArrowDaily;
    [SerializeField] GameObject ItemMission;
    [SerializeField] Transform parentItemMain;
    [SerializeField] Transform parentItemDaily;
    [SerializeField] GameObject prefabRewards;
    [SerializeField] Button btX;
    [SerializeField] Sprite rimClick;
    [SerializeField] Sprite rimUnClick;
    private Image previousImageLight = null;
    protected override void Awake()
    {
        base.Awake();
        Observer.Instance.AddObserver(ObserverKey.DistanceMisison, PositionPlayer);
    }
    private void OnDestroy()
    {
        Observer.Instance.RemoveObserver(ObserverKey.DistanceMisison, PositionPlayer);
    }
    Vector3 playerPosition;
    private void PositionPlayer(object data)
    {
        playerPosition = (Vector3)data;

    }
    private void Start()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.offsetMin = Vector2.zero;
       
        if (btX != null)
        {
            btX.onClick.AddListener(HidePanel);
        }
        if (imageDaily != null && imageMain != null)
        {
            imageMain.onClick.AddListener(ClickSetStatusMain);
            imageDaily.onClick.AddListener(ClickSetStatusDaily);
        }
    }
    private void ClickSetStatusMain()
    {
        if (parentItemMain != null)
        {
            parentItemMain.gameObject.SetActive(!parentItemMain.gameObject.activeSelf);
        }
        if (parentItemMain.gameObject.activeSelf)
        {
            checkFirstMain = true;
            if (UserDatas.user_Data.info.is_tutorial_done)
            {
                GetMissionMain(UserDatas.user_Data.info.current_id_main_mission);
            }
            imageArrowMain.transform.rotation = Quaternion.Euler(Vector3.zero);
        }
        else
        {
            imageArrowMain.transform.rotation = Quaternion.Euler(new Vector3(180,0,0));
        }

    }
    private void ClickSetStatusDaily()
    {
        if (parentItemDaily != null)
        {
            parentItemDaily.gameObject.SetActive(!parentItemDaily.gameObject.activeSelf);
        }
        if (parentItemDaily.gameObject.activeSelf)
        {
            checkFirstDaily = true;
            if (UserDatas.user_Data.info.open_mission_daily)
            {
                GetMissionDaily();
            }
            SetDefaultOnEnable(recordMissionBoardDailyFirst, parentItemDaily);
            imageArrowDaily.transform.rotation = Quaternion.Euler(Vector3.zero);
        }
        else
        {
            imageArrowDaily.transform.rotation = Quaternion.Euler(new Vector3(180, 0, 0));
        }
    }
    private void OnEnable()
    {
        checkFirstMain = true;
        Observer.Instance.Notify(ObserverKey.TransformPlayer,null);
        if (UserDatas.user_Data.info.open_mission_daily)
        {
            GetMissionDaily();
        }
        if (UserDatas.user_Data.info.is_tutorial_done&&UserDatas.user_Data.info.current_id_main_mission!=-1)
        {
            GetMissionMain(UserDatas.user_Data.info.current_id_main_mission);

        }
        else
        {
            SetDefaultOnEnable(recordMissionBoardDailyFirst, parentItemDaily);
        }
        Ultis.SetActiveCursor(true);
        GameConfig.gameBlockInput = true;

    }


    RecordMissionBoard recordMissionBoard;
    RecordMissionBoard recordMissionBoardDailyFirst;
    RecordMissionBoard recordMissionBoardMainFirst;
    private bool checkFirstDaily = true;
    private bool checkFirstMain = false;

    public void GetMissionDaily()
    {
        RecordMissionDailyInfo[] recordMissionDailyInfos = QuestManager.getRecordMissionDailyInfoArray;
        for (int i = 0; i < recordMissionDailyInfos.Length; i++)
        {
            RecordMissionDailyInfo item = recordMissionDailyInfos[i];
            recordMissionBoard.mission_id = item.mission_id;
            recordMissionBoard.mission_name = item.mission_name;
            recordMissionBoard.mission_type_name = item.mission_type_name;
            recordMissionBoard.mission_place = item.mission_place;
            recordMissionBoard.mission_detail = item.mission_detail;
            recordMissionBoard.current_progress = item.current_progress;
            recordMissionBoard.target_position = item.target_position;
            recordMissionBoard.distance = SetDistance(new Vector3(item.target_position[0], item.target_position[1], item.target_position[2]));
            if (UserDatas.missionDailyInfo != null)
            {
                int length = UserDatas.missionDailyInfo.Length;
                for (int j = 0; j < length; j++)
                {
                    if (recordMissionBoard.mission_id == UserDatas.missionDailyInfo[j].mission_id && !string.IsNullOrEmpty(UserDatas.missionDailyInfo[j].mission_name))
                    {
                        CheckListDid(recordMissionBoard, parentItemDaily);
                    }
                }
            }

        }
        imageArrowDaily.transform.rotation = Quaternion.Euler(new Vector3(180, 0, 0));
        imageArrowDaily.transform.rotation = Quaternion.Euler(new Vector3(180, 0, 0));
    }
    private void CheckListDid(RecordMissionBoard recordMissionBoard, Transform parentItemDaily)
    {
        if (UserDatas.missionDailyInfoUsed != null)
        {
            int length = UserDatas.missionDailyInfoUsed.Length;
            for (int i = 0; i < length; i++)
            {
                if (!string.IsNullOrEmpty(UserDatas.missionDailyInfoUsed[i].mission_name))
                {
                    if (recordMissionBoard.mission_id == UserDatas.missionDailyInfoUsed[i].mission_id)
                    {
                        ItemMission itemMission = CreateMission(recordMissionBoard, parentItemDaily);
                        itemMission.txDistance.text = "<color=#00B3FF>Completed</color>";
                        itemMission.actionClick = (data) =>
                        {
                            Debug.LogError("Hoan thanh roi");
                        };
                        break;
                    }
                }
                else
                {
                    CreateMission(recordMissionBoard, parentItemDaily);
                    break;
                }
               
            }
        }
        else
        {
            CreateMission(recordMissionBoard, parentItemDaily);
        }

    }
    private int SetDistance(Vector3 missionPosition)
    {
        int distance = (int) Vector3.Distance(missionPosition, playerPosition);
        return distance;
    }
    public void GetMissionMain(int current_quest_main)
    {
        if (current_quest_main == 0 && UserDatas.user_Data.info.open_mission_daily) { } else
        {
            RecordMissionInteractionInfo item = QuestManager.getRecordMissionInteractionInfo(current_quest_main, false);
            recordMissionBoard.mission_id = item.mission_id;
            recordMissionBoard.mission_name = item.mission_name;
            recordMissionBoard.mission_type_name = item.mission_type_name;
            recordMissionBoard.mission_place = item.mission_place;
            recordMissionBoard.mission_detail = item.mission_detail;
            recordMissionBoard.current_progress = item.current_progress;
            recordMissionBoard.distance = SetDistance(new Vector3(item.target_position[0], item.target_position[1], item.target_position[2]));
            CreateMission(recordMissionBoard, parentItemMain);
            SetDefaultOnEnable(recordMissionBoardMainFirst, parentItemMain);
        };

    }
    private ItemMission CreateMission(RecordMissionBoard recordMissionBoard, Transform parentItem)
    {
        ItemMission itemMission = CreateController.instance.CreateObjectGetComponent<ItemMission>(ItemMission, Vector3.zero, parentItem);
        itemMission.SetInforMission(recordMissionBoard);
        itemMission.actionClick = (data) => {
            SetInforValue(data);
            itemMission.imageLighting.sprite = rimClick;
            if (previousImageLight != null&&previousImageLight!=itemMission.imageLighting)
            {
                previousImageLight.sprite = rimUnClick;
            }
            previousImageLight = itemMission.imageLighting;
        };
        if(checkFirstMain)
        {
            recordMissionBoardMainFirst = recordMissionBoard;
            checkFirstMain = false;
        }
        if (checkFirstDaily)
        {
            recordMissionBoardDailyFirst = recordMissionBoard;
            checkFirstDaily = false;
        }
        return itemMission;
    }
    private void SetInforValue(RecordMissionBoard recordMissionBoard)
    {
        InformationMission informationMission = PanelManager.Show<InformationMission>();
        informationMission.SetInforValueMission(recordMissionBoard);
        informationMission.actionClickGo = (data) =>
        {
            Vector3 target = new Vector3(recordMissionBoard.target_position[0], recordMissionBoard.target_position[1], recordMissionBoard.target_position[2]);
            QuestManager.ChangeDirectionTarget(target);
        };
       
    }
    private void SetDefaultOnEnable(RecordMissionBoard recordMissionBoard, Transform parent)
    {
        int numberImage = parent.childCount;
        if (numberImage != 0)
        {
            Image imageFirst = parent.GetChild(0).GetComponent<Image>();
            if (previousImageLight != null) previousImageLight.sprite = rimUnClick;
            imageFirst.sprite = rimClick;
            previousImageLight = imageFirst;
            SetInforValue(recordMissionBoard);
        }

    }
    private void OnDisable()
    {
        if (parentItemDaily != null && parentItemMain != null)
        {
            parentItemDaily.gameObject.SetActive(true);
            parentItemMain.gameObject.SetActive(true);
        }
        Ultis.SetActiveCursor(true);
        GameConfig.gameBlockInput = true;
    }
    //RecordItemReward[] recordItemReward;
    //public void PostReward(int mission_id, string type)
    //{
    //    string token = UserDatas.token;
    //    string address = UserDatas.user_Data.info.address;
    //    TPRLAPI.instance.PostRewardResponse(token, address, type, mission_id, (data) =>
    //    {
    //        PopUpRewardMissionManager popUpRewardMissionManager = PanelManager.Show<PopUpRewardMissionManager>();
    //        RecordUserRewardResponse recordUserRewardResponse = JsonUtility.FromJson<RecordUserRewardResponse>(data);
    //        int max = recordUserRewardResponse.reward.Length;
    //        for (int i = 0; i < max; i++)
    //        {
    //            var itemlocal = recordItemReward[i];
    //            var itemGlobal = recordUserRewardResponse.reward[i];
    //            itemlocal.id_item = itemGlobal.item_id;
    //            itemlocal.itemType = itemGlobal.item_type;
    //            itemlocal.quantity = itemGlobal.quantity;
    //            ItemPopUpReward item = CreateController.instance.CreateObjectGetComponent<ItemPopUpReward>(prefabRewards, Vector3.zero, popUpRewardMissionManager.parentImage);
    //            RecordItemInventory recordItemInventory = getRecordItemInventoryByID(itemlocal.id_item.ToString());
    //            itemlocal.icon = recordItemInventory.icon;
    //            item.SetInforImageReward(itemlocal);
    //        }
    //    });

    //}
    //private static RecordItemInventory _recordItemInventory;
    //public static RecordItemInventory getRecordItemInventoryByID( string key)
    //{
    //    RecordItemInventory recordItemInventory = DataController.Instance.Mission_VO.GetDataByName<RecordItemInventory>("ItemInfo", key) ;
    //    _recordItemInventory = recordItemInventory;
    //    return _recordItemInventory;
    //}

}