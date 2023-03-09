using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[UIPanelPrefabAttr("PopupCountdown", "PopupCanvas")]
public class PopupCountdown : BasePanel
{
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private TMP_Text distanceText;

    [SerializeField] private Animator warningTextAnimator;
    public void SetCountdownText(int time)
    {
        int hh = (int)time / 3600;
        int mm = (int)(time - 3600 * hh) / 60;
        int ss = (int)(time - 3600 * hh - 60 * mm);
        countdownText.text = (hh == 0 ?"" : (hh + ":")) + (mm < 10 ? "0" + mm : mm) + ":" + (ss < 10 ? "0" + ss : ss);
        if (time <= 10)
        {
            warningTextAnimator.SetTrigger("warning");
        }
    }
    public void SetDistanceText(string distance)
    {
        distanceText.text = distance;
    }
}
