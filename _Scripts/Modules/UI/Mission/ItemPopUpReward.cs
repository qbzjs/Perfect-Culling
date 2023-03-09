using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemPopUpReward : MonoBehaviour
{
    [SerializeField] Image imItem;
    [SerializeField] TMP_Text amount;
    public void SetInforImageReward(RecordItemReward recordItemReward)
    {
        LoadIcon(recordItemReward.icon);
        amount.text = recordItemReward.quantity.ToString();

    }
    private void LoadIcon(string icon_name)
    {
        if (imItem != null)
        {
            imItem.sprite = TexturesManager.Instance.GetSprites(icon_name);
        }
    }

}
