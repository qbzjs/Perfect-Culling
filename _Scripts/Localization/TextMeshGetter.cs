using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
[RequireComponent(typeof(TextMeshProUGUI))]
public class TextMeshGetter : TextGetter
{
    private TextMeshProUGUI txMessage;

    protected override void Awake()
    {
        base.Awake();
        txMessage = GetComponent<TextMeshProUGUI>();
    }
    public override void SetText(string key)
    {
        base.SetText(key);
        if (string.IsNullOrEmpty(key)) return;
        if (txMessage != null)
        {
            string des = string.Format(Lang.GetText(key));
            if (des != null)
                txMessage.text = des;
        }
    }
    public override void SetText(string key, string[] prms)
    {
        base.SetText(key, prms);
        if (string.IsNullOrEmpty(key)) return;
        if (txMessage != null)
        {
            string des = string.Format(Lang.GetText(key), prms);
            if (des != null)
                txMessage.text = des;
        }
    }
}
