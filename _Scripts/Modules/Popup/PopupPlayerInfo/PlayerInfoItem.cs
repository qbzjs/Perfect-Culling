using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInfoItem : MonoBehaviour
{
    [SerializeField] private TMP_Text ID;
    [SerializeField] private TMP_Text playerName;
    public void SetInfo(string id,string name)
    {
        ID.text = id;
        playerName.text = name;
    }
}
