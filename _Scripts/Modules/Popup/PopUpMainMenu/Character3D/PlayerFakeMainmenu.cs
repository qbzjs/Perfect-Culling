//using UnityEditor.Animations;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using System;

public class PlayerFakeMainmenu : MonoBehaviour
{
    private Vector3 origin_Camera;
    [SerializeField]
    private GameObject ob_Camera;
    private GameObject player = null;
    private int currentCharacterId = -1;
    private Animator animator = null;
    private Dictionary<string, GameObject[]> lstCurrentItemEquiped = new Dictionary<string, GameObject[]>();
    private GameObject obModelMesh = null;
    [SerializeField] private TMP_Text tx_NameOfItem;
    private CharacterRig characterRig;
    private string[] typeEach;
    private GameObject[] prefabEachs;
    private Transform vehicle;
    private void Awake()
    {
        Observer.Instance.AddObserver(ObserverKey.EquipItem, EquipItem);
    }
    private void Start()
    {
        origin_Camera = ob_Camera.transform.localPosition;

    }
    private void OnDestroy()
    {
        Observer.Instance.RemoveObserver(ObserverKey.EquipItem, EquipItem);
    }

    private void OnEnable()
    {
        //int id = UserDatas.user_Data.info.current_selected_character;
        int id = 10;
        if (currentCharacterId == id && player != null) return;
        RecordMainmenuCharacter3D recordMainmenuCharacter3D = DataController.Instance.mainmenuCharacter3DVO.GetDataByName<RecordMainmenuCharacter3D>("MainmenuCharacter3DRenderInfo", $"{id}");
        GameObject ob = AssetBundleDownloader.GetAsset<GameObject>(BundleName.MODELS, recordMainmenuCharacter3D.name_model);
        GameObject hips = PrefabsManager.Instance.GetAsset<GameObject>("Hips");
        if (ob == null) return;
        if (player != null)
            Destroy(player);
        currentCharacterId = id;
        player = CreateController.instance.CreateObject(ob, Vector3.zero, transform);

        if (player != null)
        {
            float[] position = recordMainmenuCharacter3D.position;
            player.transform.localPosition = new Vector3(position[0], position[1] + 0.2f, position[2]);
            float scale = recordMainmenuCharacter3D.scale;
            player.transform.localScale = new Vector3(scale, scale, scale);
            animator = player.AddComponent<Animator>();
            RuntimeAnimatorController animatorController = AssetBundleDownloader.GetAsset<RuntimeAnimatorController>(BundleName.Animations, recordMainmenuCharacter3D.animator_controller);
            characterRig = CreateController.instance.CreateObjectGetComponent<CharacterRig>(hips, Vector3.zero, player.transform);
            obModelMesh = characterRig.gameObject;
            vehicle = PrefabsManager.Instance.GetChildrentByName(player, "vehicle").transform;
            int length = Enum.GetValues(typeof(InventoryItemType)).Length;
            for (int i = 0; i < length; i++)
            {
                InventoryItemType type = (InventoryItemType)i;
                RecordItemEquip record = UserDatas.GetRecordItemEquipTrueByType(type);
                if (record.itemType == type)
                    EquipItem(record);
            }
            animator.runtimeAnimatorController = animatorController;
        }
    }
    private void EquipItem(object data)
    {
        if (data == null || player == null) return;
        RecordItemEquip recordItemEquip = (RecordItemEquip)data;
        string item_id = recordItemEquip.itemID;
        Vector3 model_position = Vector3.zero;
        string key = $"{recordItemEquip.itemType}";
        InventoryItemType itemType = recordItemEquip.itemType;
        if (recordItemEquip.is_equip == false)
        {
            if (lstCurrentItemEquiped.ContainsKey(key))
            {
                GameObject[] item_removes = lstCurrentItemEquiped[key];
                int lengthItemRemove = item_removes.Length;
                for (int i = 0; i < lengthItemRemove; i++)
                {
                    Destroy(item_removes[i]);
                }
                lstCurrentItemEquiped.Remove(key);
                CheckItemEquipDefault(itemType);
                if (itemType == InventoryItemType.vehicle)
                {
                    if (tx_NameOfItem != null)
                    {
                        tx_NameOfItem.text = "";
                    }
                    animator.SetBool("isSit", false);
                    if (player != null)
                    {
                        player.transform.localPosition = model_position;
                        player.transform.rotation = Quaternion.Euler(0, 0, 0);
                    }
                    if (obModelMesh != null)
                        obModelMesh.SetActive(true);
                    ob_Camera.transform.localPosition = origin_Camera;
                }
            }
            return;
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
            case InventoryItemType.vehicle:
                file_name = "ItemVehicleInfo";
                animator.SetBool("isSit", true);
                model_position = new Vector3(0, 0.48f, 0);
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
                    typeEach = new string[1];
                    typeEach[0] = "vehicle";
                    prefabEachs = new GameObject[1];
                
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
                        ob_Camera.transform.localPosition = new Vector3(0, 1.5f, recordItemInventory.is_hide_player ? 19f : 12f);
                        parent = vehicle;
                        if (player != null)
                        {
                            player.transform.localPosition = model_position;
                            player.transform.rotation = Quaternion.Euler(0, 33f, 0);
                        }
                        if (tx_NameOfItem != null)
                        {
                            tx_NameOfItem.text = recordItemInventory.name;
                        }

                        break;
                    }
                case "hair":
                    parent = characterRig.hat;
                    break;
                case "head":
                    parent = characterRig.head;
                    break;
                case "chest":
                    parent = characterRig.chest;
                    break;
                case "low_back":
                    parent = characterRig.low_Back;
                    break;
                case "hip":
                    parent = characterRig.hip;
                    break;
                case "elbow_l":
                    parent = characterRig.elbow_Left;
                    break;
                case "elbow_r":
                    parent = characterRig.elbow_Right;
                    break;
                case "arm_l":
                    parent = characterRig.arm_Left;
                    break;
                case "arm_r":
                    parent = characterRig.arm_Right;
                    break;
                case "hand_l":
                    parent = characterRig.hand_Left;
                    break;
                case "hand_r":
                    parent = characterRig.hand_Right;
                    break;
                case "femoral_l":
                    parent = characterRig.femoral_Left;
                    break;
                case "femoral_r":
                    parent = characterRig.femoral_Right;
                    break;
                case "knee_l":
                    parent = characterRig.knee_Left;
                    break;
                case "knee_r":
                    parent = characterRig.knee_Right;
                    break;
                case "foot_l":
                    parent = characterRig.foot_Left;
                    break;
                case "foot_r":
                    parent = characterRig.foot_Right;
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
        {
            lstCurrentItemEquiped.Add(key, prefabEachs);
        }

        if (obModelMesh != null)
            obModelMesh.SetActive(!recordItemInventory.is_hide_player);
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
                if (record.itemID=="0")
                {
                    record.itemID = "0";
                    record.itemType = type;
                    record.is_equip = true;
                    EquipItem(record);
                }
            }
        //}
    }

}

