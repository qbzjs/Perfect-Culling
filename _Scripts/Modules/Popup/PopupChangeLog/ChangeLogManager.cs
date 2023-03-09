using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[UIPanelPrefabAttr("ChangeLogPopUp", "PopupCanvas")]
public class ChangeLogManager : BasePanel
{
    [SerializeField] Button btX;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.Escape, "HidePanel", HidePanel, ActionKeyType.Down);
    }
    void Start()
    {
        if (btX != null)
        {
            btX.onClick.AddListener(() =>
            {
                Ultis.SetActiveCursor(false);
                GameConfig.gameBlockInput = false;
                HidePanel();
            });
        }
    }
    private void OnDestroy()
    {
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.Escape, "HidePanel", HidePanel, ActionKeyType.Down);
    }

}
