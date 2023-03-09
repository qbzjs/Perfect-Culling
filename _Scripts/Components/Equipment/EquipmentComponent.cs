using CodeStage.AntiCheat.ObscuredTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class EquipmentComponent : MonoBehaviour
{
    private Dictionary<string, GameObject[] > lstCurrentItemEquiped = new Dictionary<string, GameObject[]>();
    private GameObject[] prefabEachs;
    [SerializeField] private Transform vehicle;
    [SerializeField] private Transform model_tf;
    private EntityManager _entityManager;
    private EntityManager entityManager
    {
        get
        {
            if (_entityManager == null)
                _entityManager = GetComponent<EntityManager>();
            return _entityManager;
        }
    }

    private Animator _animator;
    private Animator animator
    {
        get
        {
            if (_animator == null && entityManager != null)
                _animator = entityManager.animator;
            return _animator;
        }
    }

    private void RegisterEvent()
    {
        if (_isMe)
        {
            Observer.Instance.AddObserver(ObserverKey.EquipItem, CheckEquipItem);
        }
    }
    private void OnDestroy()
    {
        if (!_isMe) return;
        Observer.Instance.RemoveObserver(ObserverKey.EquipItem, CheckEquipItem);
    }
    private ObscuredBool _isMe;
    public void SetUpBehaviour(ObscuredBool isPlayer)
    {
        this._isMe = isPlayer;
        RegisterEvent();
    }

    public void CheckEquipItem(object data)
    {
        if (data == null) return;
        RecordItemEquip recordItemEquip = (RecordItemEquip)data;
        if (recordItemEquip.is_equip == false)
        {
            UnequipItem(recordItemEquip);
            return;
        }
        EquipItem(recordItemEquip);
    }

    private string[] typeEach;
    private void EquipItem(RecordItemEquip recordItemEquip)
    {
        string item_id = recordItemEquip.itemID;
        Vector3 model_position = Vector3.zero;
        string key = $"{recordItemEquip.itemType}";
        InventoryItemType itemType = recordItemEquip.itemType;
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
            case InventoryItemType.vehicle:
                if (!_isMe)
                {
                    file_name = "ItemVehicleInfo";
                    animator.SetBool("isSit", true);
                    model_position = new Vector3(0, 0.28f, 0);
                    entityManager.SetDriving(true);
                }
                else
                    Observer.Instance.Notify(ObserverKey.SetStatusEquipVehicle, recordItemEquip);
                break;
            default:
                break;
        }
        if (file_name == "") return;
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
            case InventoryItemType.vehicle:
                if (!_isMe)
                {
                    typeEach = new string[1];
                    typeEach[0] = "vehicle";
                    prefabEachs = new GameObject[1];
                }
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
                case "vehicle":
                    {
                        if (!_isMe)
                        {
                            parent = vehicle;
                            entityManager.SetActiveTextName(!recordItemInventory.is_hide_player);
                            if (model_tf != null)
                                model_tf.localPosition = model_position;
                            if (entityManager.characterRig.gameObject != null)
                                entityManager.characterRig.gameObject.SetActive(!recordItemInventory.is_hide_player);
                        }
                        break;
                    }
                case "hair":
                    parent = entityManager.characterRig.hat;
                    break;
                case "head":
                    parent = entityManager.characterRig.head;
                    break;
                case "chest":
                    parent = entityManager.characterRig.chest;
                    break;
                case "low_back":
                    parent = entityManager.characterRig.low_Back;
                    break;
                case "hip":
                    parent = entityManager.characterRig.hip;
                    break;
                case "elbow_l":
                    parent = entityManager.characterRig.elbow_Left;
                    break;
                case "elbow_r":
                    parent = entityManager.characterRig.elbow_Right;
                    break;
                case "arm_l":
                    parent = entityManager.characterRig.arm_Left;
                    break;
                case "arm_r":
                    parent = entityManager.characterRig.arm_Right;
                    break;
                case "hand_l":
                    parent = entityManager.characterRig.hand_Left;
                    break;
                case "hand_r":
                    parent = entityManager.characterRig.hand_Right;
                    break;
                case "femoral_l":
                    parent = entityManager.characterRig.femoral_Left;
                    break;
                case "femoral_r":
                    parent = entityManager.characterRig.femoral_Right;
                    break;
                case "knee_l":
                    parent = entityManager.characterRig.knee_Left;
                    break;
                case "knee_r":
                    parent = entityManager.characterRig.knee_Right;
                    break;
                case "foot_l":
                    parent = entityManager.characterRig.foot_Left;
                    break;
                case "foot_r":
                    parent = entityManager.characterRig.foot_Right;
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
        if (!lstCurrentItemEquiped.ContainsKey(key))
            lstCurrentItemEquiped.Add(key, prefabEachs);
    }
    private void UnequipItem(RecordItemEquip recordItemEquip)
    {
        string item_id = recordItemEquip.itemID;
        Vector3 model_position = Vector3.zero;
        string key = $"{recordItemEquip.itemType}";
        InventoryItemType itemType = recordItemEquip.itemType;
        if (lstCurrentItemEquiped.ContainsKey(key))
        {

            GameObject [] item_removes = lstCurrentItemEquiped[key];
            int lengthItemRemove = item_removes.Length;
            for (int i = 0; i < lengthItemRemove; i++)
            {
                Destroy(item_removes[i]);
            }
            lstCurrentItemEquiped.Remove(key);
            if (itemType == InventoryItemType.vehicle)
            {
                if (!_isMe)
                {
                    animator.SetBool("isSit", false);
                    if (model_tf != null)
                        model_tf.localPosition = model_position;
                    entityManager.SetDriving(false);
                    entityManager.SetActiveTextName(true);
                    if (entityManager.characterRig.gameObject != null)
                        entityManager.characterRig.gameObject.SetActive(true);
                }
                else
                {
                    Observer.Instance.Notify(ObserverKey.SetStatusEquipVehicle, recordItemEquip);
                }
            }
            CheckItemEquipDefault(recordItemEquip.itemType);
        }
    }
    private void CheckItemEquipDefault(InventoryItemType type)
    {
        //int length = Enum.GetValues(typeof(InventoryItemType)).Length;
        //for (int i = 0; i < length; i++)
        //{
        //    InventoryItemType type = (InventoryItemType)i;
        if (type == InventoryItemType.hair || type == InventoryItemType.head || type == InventoryItemType.middle || type == InventoryItemType.bottom || type == InventoryItemType.shoes)
        {
            RecordItemEquip record = new RecordItemEquip();
            record = UserDatas.GetRecordItemEquipTrueByType(type);
            if (string.IsNullOrEmpty(record.itemID))
            {
                record.itemID = "0";
                record.itemType = type;
                record.is_equip = true;
                UserDatas.SetEquipItem(record);
                CheckEquipItem(record);
            }
            else
            {
                record.itemID = "0";
                record.itemType = type;
                record.is_equip = false;
                UserDatas.SetEquipItem(record);
            }
        }
        //}
    }
}
