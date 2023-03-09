using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class ItemPart : MonoBehaviour
{
    [SerializeField]
    private Image icon;
    [SerializeField]
    private GameObject obSelected;
    public UnityAction<ItemPart> onClick;
    private RecordItemEquip _recordItem = default;
    public RecordItemEquip recordItem => _recordItem;
    private bool isEquiped = false;
    public UnityAction onEquipItem;
    [SerializeField] private Button bt_Click;
    private void Awake()
    {
        if (bt_Click != null)
            bt_Click.onClick.AddListener(Click);
    }
    public void Init(RecordItemInventory record_item, UnityAction<ItemPart> on_click)
    {
        _recordItem.itemID = record_item.id;
        _recordItem.is_equip = record_item.is_equip;
        _recordItem.itemType = Ultis.ParseEnum<InventoryItemType>( record_item.type);
        SetSelected(false);
        LoadIcon(record_item.icon);
        onClick = on_click;
    }

    private void LoadIcon(string icon_name)
    {
        if (icon != null)
        {
            icon.sprite = TexturesManager.Instance.GetSprites(icon_name);
        }
    }

    public void SetSelected(bool is_selected)
    {
        if (obSelected != null)
            obSelected.SetActive(is_selected);
    }



    private void Click()
    {
        Observer.Instance.Notify(ObserverKey.EquidPart, _recordItem);
        onClick?.Invoke(this);
    }

}
