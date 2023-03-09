using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
public class TradeCenterManager : MonoBehaviour
{


    [SerializeField] private List<ParagonInfoShowUp> list_ParagonShelf;

    private void Start()
    {
        if (list_ParagonShelf != null && list_ParagonShelf.Count > 0)
        {
            for (int i = 0; i < list_ParagonShelf.Count; i++)
            {
                list_ParagonShelf[i].CreateNewParagon();
            }
        }
    }
}


