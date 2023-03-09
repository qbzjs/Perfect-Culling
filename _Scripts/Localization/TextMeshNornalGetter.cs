using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
[RequireComponent(typeof(TextMeshPro))]
public class TextMeshNornalGetter : TextGetter
{
    private TextMeshPro txMessage;

    protected override void Awake()
    {
        base.Awake();
        txMessage = GetComponent<TextMeshPro>();
    }
    public override void SetText(string key)
    {
        base.SetText(key);
        if (string.IsNullOrEmpty(key)) return;
        if (txMessage != null)
        {
            string des = string.Format(Lang.GetText(key));
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
            txMessage.text = des.Replace("\\n",
                Environment.NewLine);
        }
    }
}
