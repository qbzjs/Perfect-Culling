using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
[UIPanelPrefabAttr("PopUpKeyControl","PopupCanvas")]
public class PopUpKeyControlManager : BasePanel
{
    [SerializeField] private Button buttonX;
    public UnityAction action = null;
    // Start is called before the first frame update
    void Start()
    {
        if (buttonX != null)
        {
            buttonX.onClick.AddListener(HidePanel);
        }
    }
    public override void HidePanel()
    {
        action?.Invoke();
        base.HidePanel();
    }


}
