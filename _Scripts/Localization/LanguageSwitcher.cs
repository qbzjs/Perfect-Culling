using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageSwitcher : MonoBehaviour
{
    [SerializeField]
    Button buttonVN, buttonEN, buttonFR, buttonKR;

    private void Start()
    {
        buttonVN.onClick.AddListener(() => ChangeLanguage("vn"));
        buttonEN.onClick.AddListener(() => ChangeLanguage("en"));
        buttonFR.onClick.AddListener(() => ChangeLanguage("fr"));
        buttonKR.onClick.AddListener(() => ChangeLanguage("kr"));
    }

    void ChangeLanguage(string pickedLang)
    {
        Lang.LANG = pickedLang;
    }
}
