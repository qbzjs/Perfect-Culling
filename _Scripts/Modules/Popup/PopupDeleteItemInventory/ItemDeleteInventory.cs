using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class ItemDeleteInventory : MonoBehaviour
{
    [SerializeField]
    private Image icon;
    [SerializeField]
    private TextMeshProUGUI txAmount;
    private RecordItemInventory _recordItem = default;
    public RecordItemInventory recordItem => _recordItem;

    public void Init(RecordItemInventory record)
    {
        _recordItem = record;
        LoadIcon(record.icon);
        SetAmount(record.number_delete);
    }

    private void LoadIcon(string icon_name)
    {
        if (icon != null)
        {
            icon.sprite = TexturesManager.Instance.GetSprites(icon_name);
        }
    }

    private void SetAmount(int amount)
    {
        if (txAmount != null)
            txAmount.text = $"{amount}";
    }
}
