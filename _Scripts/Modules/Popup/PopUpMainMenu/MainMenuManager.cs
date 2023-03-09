using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[UIPanelPrefabAttr("MainMenu", "PopupCanvas")]
public class MainMenuManager : BasePanel
{

    [SerializeField]
    private Button buttonX;
    private EntityManager _entityManagerFake = null;

    private void Start()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.offsetMin = Vector2.zero;
        if (buttonX != null){
             buttonX.onClick.AddListener(()=> {
            Ultis.SetActiveCursor(false);
            GameConfig.gameBlockInput = false;
            HidePanel();
        });
        }
    }
}
