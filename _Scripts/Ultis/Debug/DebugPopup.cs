using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[UIPanelPrefabAttr("Panel/Debug/DebugPopup", "Popup")]
public class DebugPopup : MonoBehaviour
{
    [SerializeField] private Button btnClose;
    [SerializeField] private Text txtContent;

    private void Awake()
    {
        PanelRoot.Register(this);
        btnClose.onClick.AddListener(HidePanel);
    }

    public void SetContent(string contentString)
    {
        txtContent.text = contentString;
    }

    public void HidePanel()
    {
        PanelRoot.Hide(this);
    }
}
