using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class TextGetter : MonoBehaviour
{
    public string lang_key;
    public string[] lang_params;
    protected virtual void Awake()
    {
        Observer.Instance.AddObserver(ObserverKey.LanguageChanged, LanguageChanged);
    }

    // Use this for initialization
    void Start()
    {
        SetText(lang_key, lang_params);
    }

    private void LanguageChanged(object data)
    {
        SetText(lang_key, lang_params);
    }
    public virtual void SetText(string key)
    {
        lang_key = key;
    }
    public virtual void SetText(string key, string[] prms)
    {
        lang_key = key;
        lang_params = prms;
    }

    public string GetKey()
    {
        return lang_key;
    }

    protected void OnDestroy()
    {
        Observer.Instance.RemoveObserver(ObserverKey.LanguageChanged, LanguageChanged);
    }
}
