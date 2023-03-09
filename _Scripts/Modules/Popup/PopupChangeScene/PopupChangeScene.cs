using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[UIPanelPrefabAttr("PopupChangeScene", "CanvasFPS")]
public class PopupChangeScene : BasePanel
{
    [SerializeField]
    private TextMeshProUGUI txProgress;
    [SerializeField]
    private Image imgProgress;
    [SerializeField]
    private CanvasGroup canvasGroup;
    public Action downloadDone = null;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.offsetMin = Vector2.zero;
    }

    private void OnEnable()
    {
        SetTextProgress("Loading...");
        SetImageFillAmount(0);
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1;
        }
    }

    private void OnDisable()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1;
            canvasGroup.LeanAlpha(0, 0.3f).setOnComplete(HidePanel);
        }
    }

    public void Hide()
    {
        OnDisable();
    }

    public void Init(float total_downloaded, int total_download)
    {
        float amount = total_downloaded / total_download;
        float totalValue = amount * 100;
        SetImageFillAmount(amount);
        string downloadPercent = $"Downloading resources : {total_downloaded}/{total_download} ({String.Format("{0:0.00}", totalValue)}%)";
        SetTextProgress(downloadPercent);
        if (total_downloaded >= total_download)
        {
            downloadDone?.Invoke();
        }
    }

    private void SetImageFillAmount(float amount)
    {
        if (imgProgress != null)
            imgProgress.fillAmount = amount;
    }

    private void SetTextProgress(string download_percent)
    {
        if (txProgress != null)
            txProgress.text = download_percent;
    }
}
