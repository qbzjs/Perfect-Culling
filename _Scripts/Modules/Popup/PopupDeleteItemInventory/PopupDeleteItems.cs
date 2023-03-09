using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[UIPanelPrefabAttr("PopupDeleteItemInventory", "PopupCanvas")]
public class PopupDeleteItems : BasePanel
{
    private ItemPoolManager _item_pool;
    private ItemPoolManager itemPool
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

    [SerializeField]
    private Transform parentItem;
    private List<ItemPool> lstDelete = new List<ItemPool>();
    private List<RecordItemInventory> lstRecordDelete = new List<RecordItemInventory>();
    [SerializeField]
    private Button btConfirm, btClose;
    private UnityAction onDone;

    protected override void Awake()
    {
        base.Awake();
        if (btConfirm != null)
            btConfirm.onClick.AddListener(ClickConfirm);
        if (btClose != null)
            btClose.onClick.AddListener(HidePanel);
    }

    private void Start()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.offsetMin = Vector2.zero;
    }

    public void Init(RecordItemInventory[] record_items, UnityAction on_done)
    {
        onDone = on_done;
        Clears();
        RecordItemInventory[] _recordItemInventories = record_items;
        if (_recordItemInventories != null && _recordItemInventories.Length > 0)
        {
            int item_length = _recordItemInventories.Length;
            for (int i = 0; i < item_length; i++)
            {
                RecordItemInventory record_item = _recordItemInventories[i];
                if (record_item.amount <= 0) continue;
                ItemPool item_manager = itemPool.GetInstance();
                item_manager.transform.SetParent(parentItem);
                item_manager.transform.localPosition = Vector3.zero;
                item_manager.transform.localScale = Vector3.one;
                item_manager.gameObject.SetActive(true);
                ItemDeleteInventory itemInventory = item_manager.GetComponent<ItemDeleteInventory>();
                itemInventory.Init(record_item);
                lstDelete.Add(item_manager);
                lstRecordDelete.Add(record_item);
            }
        }
    }

    private void Clears()
    {
        if (lstDelete != null && lstDelete.Count > 0)
        {
            int length = lstDelete.Count;
            for (int i = 0; i < length; i++)
            {
                itemPool.ReturnToPool(lstDelete[i]);
            }
            lstDelete.Clear();
        }
        lstRecordDelete.Clear();
    }

    private void ClickConfirm()
    {
        DeleteItems();
        //PopUpNotice popUpNotice = PanelManager.Show<PopUpNotice>();
        //popUpNotice.OnSetTextTwoButtonCustom(Constant.NOTICE, "Confirm delete items ?", DeleteItems, null, "Yes", "No");
    }

    private void DeleteItems()
    {
        int length = lstRecordDelete.Count;
        for (int i = 0; i < length; i++)
        {
            RecordItemInventory record = lstRecordDelete[i];
            int new_amount = record.amount - record.number_delete;
            if (new_amount < 0) new_amount = 0;
            record.amount = new_amount;
            InventoryItemType type = Ultis.ParseEnum<InventoryItemType>(record.type);
            List<RecordItemInventory> _recordItemInventories = new List<RecordItemInventory>(UserDatas.GetRecordItemInventoriesByType(type));
            if (_recordItemInventories != null && _recordItemInventories.Count > 0)
            {
                int length_record = _recordItemInventories.Count;
                for (int j = 0; j < length_record; j++)
                {
                    RecordItemInventory record_item = _recordItemInventories[j];
                    if (!record_item.id.Equals(record.id)) continue;
                    if (record_item.is_equip)
                    {
                        record_item.is_equip = false;
                        RecordItemEquip recordItemEquip = new RecordItemEquip();
                        recordItemEquip.itemID = record_item.id;
                        recordItemEquip.itemType = Ultis.ParseEnum<InventoryItemType>(record_item.type);
                        recordItemEquip.is_equip = false;
                        Observer.Instance.Notify(ObserverKey.EquipItem, recordItemEquip);
                    }
                    if (new_amount <= 0)
                    {
                        _recordItemInventories.RemoveAt(i);
                    }
                    else
                    {
                        record_item.amount = new_amount;
                        _recordItemInventories[j] = record_item;
                    }

                    break;
                }
            }
            UserDatas.SetRecordItemInventories(type, new List<RecordItemInventory>(_recordItemInventories));
        }
        onDone?.Invoke();
        HidePanel();
    }
}
