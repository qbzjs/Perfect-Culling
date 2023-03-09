using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
[RequireComponent(typeof(Text))]
public class TextNormalGetter : TextGetter
{
    private Text txMessage;


    protected override void Awake()
    {
        base.Awake();
        txMessage = GetComponent<Text>();
    }

    public  void SetColor(Color32 _color)
    {
        txMessage.color = _color;
    }

    public override void SetText(string key)
    {
        base.SetText(key);
        if (string.IsNullOrEmpty(key)) return;
        if (txMessage != null)
        {
            string tmp = Lang.GetText(key);
            string des = string.Format(Lang.GetText(key));
            txMessage.text = des;
        }
    }
    public override void SetText(string key, string[] prms)
    {
        base.SetText(key,prms);
        if (string.IsNullOrEmpty(key)) return;
        if (txMessage != null)
        {
            string des = Lang.GetText(key);
            if (prms != null && prms.Length > 0 ) 
            des = string.Format(Lang.GetText(key), prms);
            txMessage.text = des.Replace("\\n",
                Environment.NewLine);
        }
    }
}
