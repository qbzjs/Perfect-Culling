using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

[UIPanelPrefabAttr("PopUpNotice", "CanvasFPS")]
public class PopUpNotice : BasePanel
{
    [SerializeField] private TMP_Text tx_title;
    [SerializeField] private TMP_Text tx_content;
    [SerializeField] private Button bt_ok;
    [SerializeField] private TMP_Text bt_ok_text;
    [SerializeField] private Button bt_cancel;
    [SerializeField] private TMP_Text bt_cancel_text;



    public Action actionOk;
    public Action actionCancel;

    private void Start()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.offsetMin = Vector2.zero;

        if (bt_ok != null)
            bt_ok.onClick.AddListener(OnOkButtonClick);

        if (bt_cancel != null)
            bt_cancel.onClick.AddListener(OnCancelButtonClick);


    }

    private void SetTextForContent(string text)
    {
        string content = "";
        if (!string.IsNullOrEmpty(text))
            content = text;
        if (tx_content != null)
            tx_content.text = content;
    }

    private void SetTitle(string text = "")
    {
        string title = Constant.NOTICE;
        if (!string.IsNullOrEmpty(text))
            title = text;
        tx_title.text = title;
    }

    private void SetButtonMode1(bool mode1)
    {
        if (bt_ok != null)
            bt_ok.gameObject.SetActive(true);

        if (bt_cancel != null)
            bt_cancel.gameObject.SetActive(!mode1);
    }

    public void OnSetTextOneButton(string title, string content, Action actionOk = null, string buttonOkText = "")
    {
        SetTitle(title);
        SetButtonMode1(true);
        this.actionOk = actionOk;
        SetTextForContent(content);
    }


    public void OnSetTextTwoButton(string title, string content, Action actionOk = null, Action actionCancel = null)
    {
        SetTitle(title);
        SetButtonMode1(false);
        this.actionCancel = actionCancel;
        this.actionOk = actionOk;
        SetTextForContent(content);
    }

    public void OnSetTextTwoButtonCustom(string title, string content, Action actionOk = null, Action actionCancel = null, string buttonOkText = "", string buttonCancelText = "")
    {
        SetTitle(title);
        SetButtonMode1(false);
        this.actionCancel = actionCancel;
        this.actionOk = actionOk;
        SetTextForContent(content);
        if (!string.IsNullOrEmpty(buttonOkText)) bt_ok_text.text = buttonOkText;
        if (!string.IsNullOrEmpty(buttonCancelText)) bt_cancel_text.text = buttonCancelText;
    }

    public void OnOkButtonClick()
    {
        actionOk?.Invoke();
        HidePanel();
    }
    public void OnCancelButtonClick()
    {
        actionCancel?.Invoke();
        HidePanel();
    }
}
