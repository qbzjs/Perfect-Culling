using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

[UIPanelPrefabAttr("PopUpReward", "PopupCanvas")]
public class PopUpReward : BasePanel
{
    [SerializeField] private TMP_Text tx_title;
    [SerializeField] private GameObject itemRewardPrefab;
    [SerializeField] private Button bt_ok;
    [SerializeField] private TMP_Text bt_ok_text;
    [SerializeField] private Transform tfParentReward;
    public Action actionOk;

    private void Start()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.offsetMin = Vector2.zero;

        if (bt_ok != null)
            bt_ok.onClick.AddListener(OnOkButtonClick);
    }

    private void SetTitle(string text = "")
    {
        string title = "";
        if (!string.IsNullOrEmpty(text))
            title = text;
        tx_title.text = title;
    }

    private void SetTextButton(string text = "Claim")
    {
        string title = "";
        if (!string.IsNullOrEmpty(text))
            title = text;
        bt_ok_text.text = title;
    }

    private void CreateRewards(RecordReward[] rewards)
    {
        if (rewards == null || rewards.Length == 0) return;
        int length = rewards.Length;
        for (int i = 0; i < length; i++)
        {
            GameObject ob = Instantiate(itemRewardPrefab, Vector3.zero, Quaternion.identity, tfParentReward);
            ob.transform.localScale = Vector3.one;
            ob.transform.localPosition = new Vector3(ob.transform.localPosition.x, ob.transform.localPosition.y, 0);
            ob.SetActive(true);
            ItemPopupReward reward = ob.GetComponent<ItemPopupReward>();
            reward.SetInfo(rewards[i]);
        }
    }

    public void SetContent(string title, RecordReward[] rewards, Action actionOk = null, string buttonOkText = "Claim")
    {
        this.actionOk = actionOk;
        SetTitle(title);
        SetTextButton(buttonOkText);
        CreateRewards(rewards);
    }

    public void OnHidePopUp()
    {
        Destroy(gameObject);
    }

    public void OnOkButtonClick()
    {
        actionOk?.Invoke();
        OnHidePopUp();
    }
}


[System.Serializable]
public struct RecordReward
{
    public int id;
    public string name;
    public string type;
    public string icon;

    public RecordPrefab prefab;
    [System.Serializable]
    public struct RecordPrefab
    {
        public string name;
        public string parent;
        public float [] position;
    }
    public int amount;
}
