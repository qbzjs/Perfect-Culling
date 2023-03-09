using CodeStage.AntiCheat.ObscuredTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[UIPanelPrefabAttr("PopupPlayerInfo", "PopupCanvas")]
public class PopupPlayerInfo : BasePanel
{
    [SerializeField] private GameObject playerInfoItem;
    [SerializeField] private GameObject content;
    private Dictionary<ObscuredString, GameObject> dictionary = new Dictionary<ObscuredString, GameObject>();
    public void Init(List<NetworkedEntityState> lstPlayers)
    {
        foreach(NetworkedEntityState networkedEntityState in lstPlayers)
        {
            if (dictionary.ContainsKey(networkedEntityState.id)){
                continue;
            }
            GameObject playerInfo = Instantiate(playerInfoItem, content.transform);
            PlayerInfoItem item = playerInfo.GetComponent<PlayerInfoItem>();
            item.SetInfo(networkedEntityState.id, networkedEntityState.username);
            dictionary.Add(networkedEntityState.id, playerInfo);
        }
    }
    public void OnAddUser(string key, NetworkedEntityState value)
    {
        GameObject playerInfo = Instantiate(playerInfoItem, content.transform);
        PlayerInfoItem item = playerInfo.GetComponent<PlayerInfoItem>();
        item.SetInfo(key, value.username);
        dictionary.Add(key, playerInfo);
    }
    public void OnRemoveUser(string key, NetworkedEntityState value)
    {
        if (dictionary.ContainsKey(key))
        {
            Destroy(dictionary[key]);
            dictionary.Remove(key);
        }
    }
} 
