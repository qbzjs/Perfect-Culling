using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemPopupReward : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txName;
    [SerializeField] private TextMeshProUGUI txAmount;
    [SerializeField] private Image icon;

    public void SetInfo(RecordReward reward)
    {
        if (txName != null)
            txName.text = reward.name;
        if (txAmount != null)
            txAmount.text = reward.amount <= 0 ? "" : $"{reward.amount}";
        if (icon != null)
        {
            icon.sprite = Resources.Load<Sprite>($"Sprites/NPC {reward.icon}");
            icon.color = new Color(255, 255, 255, 255);
        }
    }
}
