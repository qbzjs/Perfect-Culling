using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Net;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine.Events;

public class ItemMission : MonoBehaviour
{
    [SerializeField] private TMP_Text txMissionTypeName;
    public TMP_Text txDistance;
    public Image imageLighting;
    [SerializeField] private Button btMission;

    public UnityAction<RecordMissionBoard> actionClick =null;

    public void SetInforMission(RecordMissionBoard recordMissionBoard)
    {
        txMissionTypeName.text = recordMissionBoard.mission_type_name;
        txDistance.text = recordMissionBoard.distance.ToString()+"m";
        if (btMission != null)
        {
            btMission.onClick.AddListener(()=> {
                actionClick?.Invoke(recordMissionBoard); });
        }
    }
    private void OnDisable()
    {
        Destroy(gameObject);
    }

}
