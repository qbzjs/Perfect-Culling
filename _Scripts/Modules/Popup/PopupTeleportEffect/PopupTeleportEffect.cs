using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[UIPanelPrefabAttr("PopupTeleportEffect", "CanvasPopup")]
public class PopupTeleportEffect : BasePanel
{
    [SerializeField]
    private Animator teleportEffect;
    [SerializeField]
    private TMP_Text placeNameText;

    public void PlayAnim()
    {
        teleportEffect.SetTrigger("play");
    }
    public void SetPlaceName(string place_name)
    {
        placeNameText.text = place_name;
    }
}
