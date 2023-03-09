using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DataLoader.VO;

[UIPanelPrefabAttr("MainmenuInventory", "MainmenuImageDown")]
public class MainmenuInventory : BasePanel
{
    [SerializeField]
    private Transform parentItem;
    [SerializeField]
    private ItemTapInvetory[] itemTapInvetories;
    private ItemTapInvetory currentTapInventory = null;
    private ItemInventory currentItemInventory = null;
    private ItemInventory previousItemInventory = null;
    private List<ItemPool> lstCurrentItems = new List<ItemPool>();
    [SerializeField]
    private Button btChooseItemsDelete, btDelete, btCancelDelete;
    private Dictionary<string, RecordItemInventory> lstRecordDelete = new Dictionary<string, RecordItemInventory>();

    ItemPoolManager _item_pool;
    protected ItemPoolManager itemPool
    {
        get
        {
            if (_item_pool == null)
            {
                try
                {
                    if (!TryGetComponent<ItemPoolManager>(out _item_pool))
                    {
                        _item_pool = gameObject.AddComponent<ItemPoolManager>();
                    }
                }
                catch (System.Exception ex) { }
            }
            return _item_pool;
        }
    }

    private void OnEnable()
    {
        if (itemTapInvetories != null && itemTapInvetories.Length > 0)
            ItemTapClick(itemTapInvetories[0]);
    }

    protected override void Awake()
    {
        if (btChooseItemsDelete != null)
            btChooseItemsDelete.onClick.AddListener(ClickChooseItemsDelete);
        if (btDelete != null)
            btDelete.onClick.AddListener(ClickDelete);
        if (btCancelDelete != null)
            btCancelDelete.onClick.AddListener(ClickCancelDelete);
    }

    private void EquipItem()
    {
        previousItemInventory = null;
        RecordItemEquip itemEquip = UserDatas.GetRecordItemEquipTrueByType(currentItemInventory.inventoryItemType);
        if (allCurrentItem == null||string.IsNullOrEmpty(itemEquip.itemID)) return;
        int length= allCurrentItem.Length;
        for (int i = 0; i < length; i++)
        {
            ItemInventory itemCurrent = allCurrentItem[i];
            if (itemCurrent.inventoryItemType == currentItemInventory.inventoryItemType)
            {
                if (itemCurrent.recordItem.id== itemEquip.itemID)
                {
                    previousItemInventory = itemCurrent;
                }
            }
        }
        if (previousItemInventory != null)
        {
            if (previousItemInventory == currentItemInventory) return;
            previousItemInventory.SetEquiped(false);
            RecordItemInventory record = previousItemInventory.recordItem;
            RecordItemEquip recordItem = new RecordItemEquip();
            recordItem.itemID = record.id;
            recordItem.itemType = previousItemInventory.inventoryItemType;
            recordItem.is_equip = false;
            Observer.Instance.Notify(ObserverKey.EquipItem, recordItem);
            UserDatas.SetEquipItem(recordItem);

        }
        //previousItemInventory = currentItemInventory;

    }

    private void Start()
    {
        if (itemTapInvetories != null && itemTapInvetories.Length > 0)
        {
            int length = itemTapInvetories.Length;
            for (int i = 0; i < length; i++)
            {
                itemTapInvetories[i].onClick = (tap) => { ItemTapClick(tap); };
            }
            currentTapInventory = itemTapInvetories[0];
        }
    }

    private void ItemTapClick(ItemTapInvetory tap)
    {
        if (currentTapInventory != null)
        {
            if (currentTapInventory == tap) return;
            currentTapInventory.SetActiveHighlight(false);
        }
        currentTapInventory = tap;
        currentTapInventory.SetActiveHighlight(true);
        currentItemInventory = null;
        lstRecordDelete.Clear();
        SetActiveBtDelete(false);
        SetActiveBtChooseItemsDelete(true);
        CreateItems();
    }
    private ItemInventory[] allCurrentItem; 
    private void CreateItems()
    {
        ClearItems();
        RecordItemInventory[] _recordItemInventories = currentTapInventory.recordItemInventories;
        if (_recordItemInventories != null && _recordItemInventories.Length > 0)
        {
            int item_length = _recordItemInventories.Length;
            allCurrentItem = new ItemInventory[item_length]; 
            for (int i = 0; i < item_length; i++)
            {
                RecordItemInventory record_item = _recordItemInventories[i];
                ItemPool item_manager = itemPool.GetInstance();
                item_manager.transform.SetParent(parentItem);
                item_manager.transform.localPosition = Vector3.zero;
                item_manager.transform.localScale = Vector3.one;
                if (record_item.is_hide_item)
                {
                    item_manager.gameObject.SetActive(false);
                }
                else
                {
                    item_manager.gameObject.SetActive(true);
                }
                ItemInventory itemInventory = item_manager.GetComponent<ItemInventory>();
                itemInventory.onEquipItem = EquipItem;
                if (record_item.is_equip)
                {
                    previousItemInventory = itemInventory;
                }
                itemInventory.Init(record_item, ClickItemInventory, OnChangeNumberDelete);
                if (currentTapInventory.parentTransform != null)
                {
                    int lengthTransforms = currentTapInventory.parentTransform.Length;
                    for (int indexTransform = 0; indexTransform < lengthTransforms; indexTransform++)
                    {
                        if (record_item.type == currentTapInventory.parentTransform[indexTransform].name)
                        {
                            itemInventory.transform.SetParent(currentTapInventory.parentTransform[indexTransform]);
                        }
                    }
                }
                lstCurrentItems.Add(item_manager);
                allCurrentItem[i] = itemInventory;
                item_manager.name = record_item.type;
            }
        }
    }

    private void OnChangeNumberDelete(RecordItemInventory record)
    {

        string key = $"{record.type}_{record.id}";
        if (lstRecordDelete.ContainsKey(key))
            lstRecordDelete[key] = record;
        else
            lstRecordDelete.Add(key, record);
    }

    private void ClearItems()
    {
        if (lstCurrentItems.Count == 0) return;
        int length = lstCurrentItems.Count;
        for (int i = 0; i < length; i++)
        {
            itemPool.ReturnToPool(lstCurrentItems[i]);
        }
        lstCurrentItems.Clear();
    }

    private void ClickItemInventory(ItemInventory item)
    {
        if (btCancelDelete != null && btCancelDelete.gameObject.activeSelf)
        {
            bool is_ticked_delete = item.isTickedDelete;
            RecordItemInventory record = item.recordItem;
            string key = $"{record.type}_{record.id}";
            if (is_ticked_delete)
            {
                if (!lstRecordDelete.ContainsKey(key))
                    lstRecordDelete.Add(key, record);
            }
            else
            {
                if (lstRecordDelete.ContainsKey(key))
                    lstRecordDelete.Remove(key);
            }
            SetActiveBtDelete(lstRecordDelete.Count > 0);
            return;
        }
        if (currentItemInventory != null)
        {
            //if (currentItemInventory == item) return;
            currentItemInventory.SetSelected(false);
            currentItemInventory.SetActiveUseWhenSelected(false, true);
        }
        currentItemInventory = item;
        currentItemInventory.SetSelected(true);
        currentItemInventory.SetActiveUseWhenSelected(!currentItemInventory.recordItem.is_equip);
    }

    private void ClickChooseItemsDelete()
    {
        if (lstCurrentItems.Count == 0) return;
        int length = lstCurrentItems.Count;
        for (int i = 0; i < length; i++)
        {
            lstCurrentItems[i].GetComponent<ItemInventory>().SetActiveTickBorder(true);
        }
        SetActiveBtChooseItemsDelete(false);
        if (currentItemInventory != null)
        {
            currentItemInventory.SetSelected(false);
            currentItemInventory.SetActiveUseWhenSelected(false, true);
        }
    }

    private void SetActiveBtChooseItemsDelete(bool active)
    {
        if (btChooseItemsDelete != null)
            btChooseItemsDelete.gameObject.SetActive(active);
        if (btCancelDelete != null)
            btCancelDelete.gameObject.SetActive(!active);
    }

    private void SetActiveBtDelete(bool active)
    {
        if (btDelete != null)
            btDelete.gameObject.SetActive(active);
    }

    private void ClickDelete()
    {
        if (lstRecordDelete == null || lstRecordDelete.Count == 0)
        {
            PopUpNotice popUpNotice = PanelManager.Show<PopUpNotice>();
            popUpNotice.OnSetTextOneButton(Constant.NOTICE, "Please choose at least one item to delete", null, "Understand");
            return;
        }
        PopupDeleteItems popupDeleteItems = PanelManager.Show<PopupDeleteItems>();
        popupDeleteItems.Init(new List<RecordItemInventory>(lstRecordDelete.Values).ToArray(), OnDoneDelete);
    }

    private void OnDoneDelete()
    {
        lstRecordDelete.Clear();
        currentTapInventory.recordItemInventories = null;
        CreateItems();
        SetActiveBtDelete(false);
        SetActiveBtChooseItemsDelete(true);
    }

    private void ClickCancelDelete()
    {
        lstRecordDelete.Clear();
        int length = lstCurrentItems.Count;
        for (int i = 0; i < length; i++)
        {
            lstCurrentItems[i].GetComponent<ItemInventory>().SetActiveTickBorder(false);
        }
        if (currentItemInventory != null)
        {
            currentItemInventory.SetSelected(true);
            currentItemInventory.SetActiveUseWhenSelected(!currentItemInventory.recordItem.is_equip);
        }
        SetActiveBtDelete(false);
        SetActiveBtChooseItemsDelete(true);
    }
}
