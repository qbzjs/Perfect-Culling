using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EquipCharacterParts : MonoBehaviour
{

    [SerializeField] private CharacterRig rig;
    private Dictionary<string, GameObject[]> lstCurrentPartEquiped = new Dictionary<string, GameObject[]>();
    private Dictionary<string, RecordItemEquip> lst_CurrentRecordItem = new Dictionary<string, RecordItemEquip>();
    private GameObject[] prefabEachs;
    private string[] typeEach;

    private void Awake()
    {
        Observer.Instance.AddObserver(ObserverKey.EquidPart, EquipCharacterPart);
    }

    private void Start()
    {
        CheckItemEquipDefault();
    }
    private void OnDestroy()
    {
        Observer.Instance.RemoveObserver(ObserverKey.EquidPart, EquipCharacterPart);
    }
    RecordItemEquip recordItemEquip;
    private void EquipCharacterPart(object data)
    {
        if (data == null) return;
        RecordItemEquip recordPart = (RecordItemEquip)data;
        string item_id = recordPart.itemID;
        InventoryItemType itemType = recordPart.itemType;
        string key = $"{itemType}";
        if (lst_CurrentRecordItem.ContainsKey(key))
        {
            GameObject[] item_removes = lstCurrentPartEquiped[key];
            lstCurrentPartEquiped.Remove(key);
            lst_CurrentRecordItem.Remove(key);
            foreach (var item_remove in item_removes)
            {
                Destroy(item_remove);
            }
        }
        string file_name = "";
        switch (itemType)
        {
            case InventoryItemType.hair:
                file_name = "HairCharacter";
                break;
            case InventoryItemType.head:
                file_name = "HeadCharacter";
                break;
            case InventoryItemType.middle:
                file_name = "MiddleCharacter";
                break;
            case InventoryItemType.bottom:
                file_name = "BottomCharacter";
                break;
            case InventoryItemType.shoes:
                file_name = "ShoesCharacter";
                break;
            default:
                break;
        }
        RecordItemInventory recordItemInventory = DataController.Instance.characterVO.GetDataByName<RecordItemInventory>(file_name, item_id);
        GameObject parentOb = PrefabsManager.Instance.GetAsset<GameObject>(recordItemInventory.name);
        if (parentOb == null) return;
        switch (itemType)
        {
            case InventoryItemType.hair:
                typeEach = new string[1];
                typeEach[0] = "hair";
                prefabEachs = new GameObject[1];
                break;
            case InventoryItemType.head:
                typeEach = new string[1];
                typeEach[0] = "head";
                prefabEachs = new GameObject[1];
                break;
            case InventoryItemType.middle:
                typeEach = new string[9];
                typeEach[0] = "chest";
                typeEach[1] = "low_back";
                typeEach[2] = "hip";
                typeEach[3] = "elbow_l";
                typeEach[4] = "elbow_r";
                typeEach[5] = "arm_l";
                typeEach[6] = "arm_r";
                typeEach[7] = "hand_l";
                typeEach[8] = "hand_r";
                prefabEachs = new GameObject[9];
                break;
            case InventoryItemType.bottom:
                typeEach = new string[4];
                typeEach[0] = "femoral_l";
                typeEach[1] = "femoral_r";
                typeEach[2] = "knee_l";
                typeEach[3] = "knee_r";
                prefabEachs = new GameObject[4];
                break;
            case InventoryItemType.shoes:
                typeEach = new string[2];
                typeEach[0] = "foot_l";
                typeEach[1] = "foot_r";
                prefabEachs = new GameObject[2];
                break;
            default:
                break;
        }
        Transform parent = null;
        int lengthEach = typeEach.Length;
        for (int i = 0; i < lengthEach; i++)
        {
            switch (typeEach[i])
            {
                case "hair":
                    parent = rig.hat;
                    break;
                case "head":
                    parent = rig.head;
                    break;
                case "chest":
                    parent = rig.chest;
                    break;
                case "low_back":
                    parent = rig.low_Back;
                    break;
                case "hip":
                    parent = rig.hip;
                    break;
                case "elbow_l":
                    parent = rig.elbow_Left;
                    break;
                case "elbow_r":
                    parent = rig.elbow_Right;
                    break;
                case "arm_l":
                    parent = rig.arm_Left;
                    break;
                case "arm_r":
                    parent = rig.arm_Right;
                    break;
                case "hand_l":
                    parent = rig.hand_Left;
                    break;
                case "hand_r":
                    parent = rig.hand_Right;
                    break;
                case "femoral_l":
                    parent = rig.femoral_Left;
                    break;
                case "femoral_r":
                    parent = rig.femoral_Right;
                    break;
                case "knee_l":
                    parent = rig.knee_Left;
                    break;
                case "knee_r":
                    parent = rig.knee_Right;
                    break;
                case "foot_l":
                    parent = rig.foot_Left;
                    break;
                case "foot_r":
                    parent = rig.foot_Right;
                    break;
            }
            GameObject prefabsitemEach;
            if (parentOb.transform.childCount > 0)
            {
                prefabsitemEach = PrefabsManager.Instance.GetChildrentByName(parentOb, typeEach[i]);
            }
            else
            {
                prefabsitemEach = parentOb;
            }
            if (prefabsitemEach != null)
            {
                 prefabEachs[i] = CreateController.instance.CreateObject(prefabsitemEach, Vector3.zero, parent);
            }
        }
        List<RecordItemInventory> itemInventories = UserDatas.GetRecordItemInventoriesByType(itemType);
        int count = itemInventories.Count;
        for (int i = 0; i < count; i++)
        {
            recordItemEquip.itemID = itemInventories[i].id;
            recordItemEquip.itemType = itemType;
            recordItemEquip.is_equip = false;
            UserDatas.SetEquipItem(recordItemEquip);
        }
        if (!lstCurrentPartEquiped.ContainsKey(key))
        {
            lstCurrentPartEquiped.Add(key, prefabEachs);
            lst_CurrentRecordItem.Add(key, recordPart);
            recordItemEquip.itemID = recordPart.itemID;
            recordItemEquip.itemType = itemType;
            recordItemEquip.is_equip = true;
            UserDatas.SetEquipItem(recordItemEquip);
            //UserDatas.listCurrentRig.Add(recordItemEquip);
        }
    }
    private void CheckItemEquipDefault()
    {
        int length = Enum.GetValues(typeof(InventoryItemType)).Length;
        for (int i = 0; i < length; i++)
        {
            InventoryItemType type = (InventoryItemType)i;
            if (type == InventoryItemType.hair || type == InventoryItemType.head || type == InventoryItemType.middle || type == InventoryItemType.bottom || type == InventoryItemType.shoes)
            {
                RecordItemEquip record = new RecordItemEquip();
                record = UserDatas.GetRecordItemEquipTrueByType(type);
                record.itemID = "0";
                record.itemType = type;
                record.is_equip = true;
                EquipCharacterPart(record);
            }
        }
    }

}
