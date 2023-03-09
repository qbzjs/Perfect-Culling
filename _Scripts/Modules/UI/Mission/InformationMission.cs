using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

[UIPanelPrefabAttr("ValueInformationQuest", "QuestDetail")]
public class InformationMission : BasePanel
{
    [SerializeField] private TMP_Text txTypeName;
    [SerializeField] private TMP_Text txPlace;
    [SerializeField] private TMP_Text txNameMission;
    [SerializeField] private TMP_Text txDetail;
    [SerializeField] private Button btGo;
    [SerializeField] private ItemPopUpReward item;
    public UnityAction<RecordMissionBoard> actionClickGo=null;
    private void Start()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(-220, 460);
    }
    // Start is called before the first frame update
    public void SetInforValueMission(RecordMissionBoard record)
    {
        SetText(txTypeName, record.mission_type_name);
        SetText(txPlace, record.mission_place);
        SetText(txNameMission, record.mission_name);
        SetText(txDetail, record.mission_detail);
        if (btGo != null)
        {
            btGo.onClick.AddListener(() =>
            {
                actionClickGo?.Invoke(record);
            });
        }
    }
    private void SetText(TMP_Text tx, string str)
    {
        if (tx != null)
        {
            tx.text = str;
        }
    }
}
