using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class UIQuest : MonoBehaviour
{
    private int questId;
    [SerializeField] private TMP_Text txQuestType;
    [SerializeField] private Image questImage;
    [SerializeField] private TMP_Text distanceToQuestText;
    [SerializeField] private TMP_Text questNameText;

    private Vector3 _targetPosition;
    public Vector3 targetPosition
    {
        get
        {
            return _targetPosition;
        }
        set
        {
            _targetPosition = value;
        }
    }

    public void SetDistanceToQuest(string distance)
    {
        distanceToQuestText.text = distance;
        if (popupCountdown != null)
        {
            popupCountdown.SetDistanceText(distance);
        }
    }
    public void SetQuestTypeName(string typeQuest, string typeName)
    {
        if (typeQuest == null || typeName == null) return;
        string strTypeQuest = typeQuest;
        string str1 = strTypeQuest.Substring(0, 1);
        string str2 = strTypeQuest.Substring(1);
        str1 = str1.ToUpper();
        strTypeQuest = str1 + str2;
        StringBuilder strBl = new StringBuilder();
        if(typeQuest == "main")
        {
            strBl.Append("<color=#FFB935>");
            strBl.Append("[");
            strBl.Append(strTypeQuest);
            strBl.Append("]");
            strBl.Append("</color>");
        }else if (typeQuest == "daily")
        {
            strBl.Append("<color=#8972D7>");
            strBl.Append("[");
            strBl.Append(strTypeQuest);
            strBl.Append("]");
            strBl.Append("</color>");
        }

        strBl.Append(" ");
        strBl.Append(typeName);
        txQuestType.text = strBl.ToString();
    }
    public void SetQuestName(string quest_name)
    {
        questNameText.text = quest_name;
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
    }
    public void SetQuestID(int quest_id)
    {
        questId = quest_id;
    }
    public int timeRemaining;
    PopupCountdown popupCountdown;
    public void UpdateQuestRepeating(Vector3 targetPosition,int time_limit, int delta_time)
    {
        this.targetPosition = targetPosition;
        timeRemaining = time_limit;
        popupCountdown = PanelManager.Show<PopupCountdown>();
        popupCountdown.SetCountdownText(timeRemaining);
        InvokeRepeating("SetQuestInfo",0,delta_time);
    }
    public void StopUpdateQuestRepeating()
    {
        CancelInvoke();
        HidePopupCountdown();
    }
    private void HidePopupCountdown()
    {
        PanelManager.Hide<PopupCountdown>();
    }
    void SetQuestInfo()
    {
        timeRemaining--;
        popupCountdown.SetCountdownText(timeRemaining);
        if (timeRemaining <= 0)
        {
            StopUpdateQuestRepeating();
            HidePopupCountdown();
            PopUpCompleteQuestEffect popUpCompleteQuestEffect = PanelManager.Show<PopUpCompleteQuestEffect>();
            popUpCompleteQuestEffect.PlayAnimComplete(false);
        }
    }
}
