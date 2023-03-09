using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[UIPanelPrefabAttr("PopupQuest", "PopupCanvasInteract")]
public class PopupQuest : BasePanel
{
    [SerializeField] private Transform questMainParent;
    [SerializeField] private Transform questDailyParent;
    [SerializeField] private GameObject uIQuestPrefab;
    [SerializeField] private RectTransform rtQuest;
    [SerializeField] private Button btBardly;
    [SerializeField] private Mask mask;

    private Dictionary<int, UIQuest> currentQuestList = new Dictionary<int, UIQuest>();

    private Transform playerTransform;
    private GameObject newQuest;
    private void Start()
    {
        if (btBardly != null)
        {
            btBardly.onClick.AddListener(ClickBtBoardly);
        }
    }
    private void ClickBtBoardly()
    {
        var rotationVector = btBardly.transform.rotation.eulerAngles;
        rotationVector.z = rotationVector.z + 180;
        btBardly.transform.rotation = Quaternion.Euler(rotationVector);
        if (mask != null)
        {
            mask.enabled = !mask.enabled;
        }
    }
    public bool AddQuestWithParameter(int quest_id, string quest_name, Vector3 target_position, string typeQuest, string typeName)
    {
        if (currentQuestList.ContainsKey(quest_id))
        {
            return false;
        }
        if (typeQuest == "main")
        {
            newQuest = CreateController.instance.CreateObject(uIQuestPrefab, Vector2.zero, questMainParent);
            
        }
        else if (typeQuest == "daily")
        {
            newQuest = CreateController.instance.CreateObject(uIQuestPrefab, Vector2.zero, questDailyParent);
        }
        UIQuest uIQuest = newQuest.GetComponent<UIQuest>();
        uIQuest.SetQuestName(quest_name);
        uIQuest.SetQuestID(quest_id);
        uIQuest.SetQuestTypeName(typeQuest, typeName);
        uIQuest.targetPosition = target_position;
        currentQuestList.Add(quest_id, uIQuest);
        int heightPopup = 10 + currentQuestList.Count * 40;
        if (rtQuest != null)
            rtQuest.sizeDelta = new Vector2(250, 10 + currentQuestList.Count * 40);
        if (heightPopup > 150)
        {
            if (mask != null)
                mask.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(250, 150);
        }
        else
        {
            mask.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(250, heightPopup);
        }
        return true;
    }
    public void SetPlayerTransform(Transform player_transform)
    {
        playerTransform = player_transform;
    }
    public void CompleteQuest(int quest_id)
    {
        RemoveQuest(quest_id);
    }
    private void RemoveQuest(int quest_id)
    {
        if (!currentQuestList.ContainsKey(quest_id)) return;
        UIQuest removeQuest = currentQuestList[quest_id];
        currentQuestList.Remove(quest_id);
        Destroy(removeQuest.gameObject);
    }
    private void Update()
    {
        UpdateQuestDistance();
    }
    public void UpdateQuestDistance()
    {
        foreach (KeyValuePair<int, UIQuest> quest in currentQuestList)
        {
            quest.Value.SetDistanceToQuest(((int)(playerTransform.position - quest.Value.targetPosition).magnitude) + "m");
        }
    }
    public UIQuest GetCurrentQuest(int id = -1)
    {
        if(id == -1)
        {
            id = UserDatas.user_Data.info.current_id_main_mission;
            return currentQuestList[id];
        }
        else
        {
            return currentQuestList[id];
        }

    }
}
