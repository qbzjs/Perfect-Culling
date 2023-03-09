using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[UIPanelPrefabAttr("KeyControl", "PopupCanvas")]
public class KeyControl : BasePanel
{
    [SerializeField] private Button bt_Close;
    // Start is called before the first frame update
    void Start()
    {
        if (bt_Close != null)
        {
            bt_Close.onClick.AddListener(() =>
            {
                HidePanel();
            });
        }
    }
}
