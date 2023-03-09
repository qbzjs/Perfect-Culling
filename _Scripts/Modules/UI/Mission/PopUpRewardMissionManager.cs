using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[UIPanelPrefabAttr("PopUpRewardForMission","MissionInfo")]
public class PopUpRewardMissionManager : BasePanel
{
    public GameObject itemImage;
    public Transform parentImage;
    [SerializeField] Button btOk;
    // Start is called before the first frame update
    void Start()
    {
        if (btOk != null)
        {
            btOk.onClick.AddListener(HidePanel);
        }
    }


}
