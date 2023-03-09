
using System;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using UnityEngine;

public class UserDatas
{
    private static ObscuredString _token = "";
    public static ObscuredString token
    {
        set
        {
            _token = value;
            ObscuredPrefs.Set<string>(Constant.Token, value);
            ObscuredPrefs.Save();
        }
        get
        {
            return _token;
        }
    }

    public static RecordUserData user_Data;
    public static RecordMissionDailyInfo[] missionDailyInfo;
    public static RecordMissionDailyInfo[] missionDailyInfoUsed; 
    public static bool is_guest = true;
    public static Paragons paragons;
    public static RecordItemEquip current_vehicle;
    public static RecordChat[] record_chat_history;
    public static string ow_session_id = "";
    public static CurrentRoomType current_room_type = CurrentRoomType.None;
    public static ObscuredString lastClassIdJoined = "";

    public static void AddCharacter(int id)
    {
        List<RecordCharacter> user_characters = new List<RecordCharacter>(UserDatas.user_Data.user_characters);
        bool is_contain = false;
        int length = user_characters.Count;
        for (int i = 0; i < length; i++)
        {
            if (user_characters[i].id == id)
            {
                is_contain = true;
                break;
            }
        }
        if (is_contain == false)
        {
            RecordCharacter[] _recordCharacters = DataController.Instance.characterVO.GetDatasByName<RecordCharacter>("CharactersInfo");
            user_characters.Add(Array.Find(_recordCharacters, x => x.id == id));
            UserDatas.user_Data.user_characters = user_characters.ToArray();
        }
    }

    public static void SetEquipItem(RecordItemEquip record)
    {
        List<RecordItemInventory> _recordItemInventories = GetRecordItemInventoriesByType(record.itemType);
        if (_recordItemInventories == null || _recordItemInventories.Count == 0) return;
        int length = _recordItemInventories.Count;
        for (int i = 0; i < length; i++)
        {
            if (!_recordItemInventories[i].id.Equals(record.itemID)) continue;
            RecordItemInventory record1 = _recordItemInventories[i];
            record1.is_equip = record.is_equip;
            _recordItemInventories[i] = record1;
            break;
        }
        SetRecordItemInventories(record.itemType, _recordItemInventories);
    }

    public static RecordItemEquip GetRecordItemEquipTrueByType(InventoryItemType type)
    {
     RecordItemEquip _recordCurrentItemEquip= new RecordItemEquip();
     List<RecordItemInventory> itemInventory = GetRecordItemInventoriesByType(type);
        if (itemInventory != null)
        {
            int count = itemInventory.Count;
            for (int k = 0; k < count; k++)
            {
                if (itemInventory[k].is_equip)
                {
                    _recordCurrentItemEquip.itemID = itemInventory[k].id;
                    _recordCurrentItemEquip.is_equip = itemInventory[k].is_equip;
                    _recordCurrentItemEquip.itemType = type;
                }
                
            }
        } ;
        return _recordCurrentItemEquip;
    } 
    public static List<RecordItemInventory> GetRecordItemInventoriesByType(InventoryItemType type)
    {
        List<RecordItemInventory> _recordItemInventories = null;
        switch (type)
        {
            case InventoryItemType.hair:
                _recordItemInventories = UserDatas.user_Data.inventory_hair;
                break;
            case InventoryItemType.head:
                _recordItemInventories = UserDatas.user_Data.inventory_head;
                break;
            case InventoryItemType.middle:
                _recordItemInventories = UserDatas.user_Data.inventory_middle;
                break;
            case InventoryItemType.bottom:
                _recordItemInventories = UserDatas.user_Data.inventory_bottom;
                break;
            case InventoryItemType.shoes:
                _recordItemInventories = UserDatas.user_Data.inventory_shoes;
                break;
            case InventoryItemType.mission:
                _recordItemInventories = UserDatas.user_Data.inventory_mission;
                break;
            case InventoryItemType.consumable:
                _recordItemInventories = UserDatas.user_Data.inventory_consumable;
                break;
            case InventoryItemType.vehicle:
                _recordItemInventories = UserDatas.user_Data.inventory_vehicle;
                break;
            default:
                break;
        }
        return _recordItemInventories;
    }

    public static void SetRecordItemInventories(InventoryItemType type, List<RecordItemInventory> recordItemInventories)
    {
        switch (type)
        {
            case InventoryItemType.hair:
                UserDatas.user_Data.inventory_hair = recordItemInventories;
                break;
            case InventoryItemType.head:
                 UserDatas.user_Data.inventory_head= recordItemInventories ;
                break;
            case InventoryItemType.middle:
                 UserDatas.user_Data.inventory_middle= recordItemInventories ;
                break;
            case InventoryItemType.bottom:
                 UserDatas.user_Data.inventory_bottom= recordItemInventories ;
                break;
            case InventoryItemType.shoes:
                UserDatas.user_Data.inventory_shoes = recordItemInventories;
                break;
            case InventoryItemType.mission:
                UserDatas.user_Data.inventory_mission = recordItemInventories;
                break;
            case InventoryItemType.consumable:
                UserDatas.user_Data.inventory_consumable = recordItemInventories;
                break;
            case InventoryItemType.vehicle:
                UserDatas.user_Data.inventory_vehicle = recordItemInventories;
                break;
            default:
                break;
        }
    }
}
