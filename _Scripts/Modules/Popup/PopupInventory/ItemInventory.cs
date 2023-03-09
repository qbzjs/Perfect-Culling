using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class ItemInventory : MonoBehaviour
{
    [SerializeField]
    private Image icon;
    [SerializeField]
    private TextMeshProUGUI txAmount, txAmountDelete;
    [SerializeField]
    private GameObject obSelected, obEquiped;
    [SerializeField]
    private GameObject obUse, obUnuse, obBuy;
    [SerializeField]
    private Button btClickIcon;
    [SerializeField]
    private GameObject obTickBorder, obTick;
    [SerializeField]
    private GameObject obAmount;
    public UnityAction<ItemInventory> onClick;
    private RecordItemInventory _recordItem = default;
    public RecordItemInventory recordItem => _recordItem;
    public bool isTickedDelete => obTick.activeSelf;
    [SerializeField]
    private GameObject obChooseAmount;
    private UnityAction<RecordItemInventory> onMinusPlus = null;
    [SerializeField]
    private Button btMinus, btPlus;
    private InventoryItemType _inventoryItemType;
    public InventoryItemType inventoryItemType => _inventoryItemType;
    private bool isEquiped = false;
    private bool isNotConsumable = false;
    public UnityAction onEquipItem;

    private void Awake()
    {
        if (btClickIcon != null)
            btClickIcon.onClick.AddListener(Click);
        if (obUse != null)
            obUse.GetComponent<Button>().onClick.AddListener(ClickUse);
        if (obUnuse != null)
            obUnuse.GetComponent<Button>().onClick.AddListener(ClickUnuse);
        if (obBuy != null)
            obBuy.GetComponent<Button>().onClick.AddListener(ClickBuy);
        if (btMinus != null)
            btMinus.onClick.AddListener(ClickMinus);
        if (btPlus != null)
            btPlus.onClick.AddListener(ClickPlus);
    }

    public void Init(RecordItemInventory record_item, UnityAction<ItemInventory> on_click, UnityAction<RecordItemInventory> on_minus_plus)
    {
        _recordItem = record_item;
        _recordItem.number_delete = 1;
        SetAmount(record_item.amount);
        SetEquiped(record_item.is_equip);
        SetSelected(false);
        SetActiveUseWhenSelected(false, true);
        LoadIcon(record_item.icon);
        SetActiveTickBorder(false);
        SetAmountDelete();
        onClick = on_click;
        onMinusPlus = on_minus_plus;
        _inventoryItemType = Ultis.ParseEnum<InventoryItemType>(_recordItem.type);
        isNotConsumable = _inventoryItemType != InventoryItemType.consumable && _inventoryItemType != InventoryItemType.mission;
    }

    private void LoadIcon(string icon_name)
    {
        if (icon != null)
        {
            icon.sprite = TexturesManager.Instance.GetSprites(icon_name);
        }
    }

    public void SetAmount(int amount)
    {
        if (obAmount != null)
            obAmount.SetActive(amount > 1 ? true : false);
        if (txAmount != null)
        {
            txAmount.text = $"{amount}";
        }
    }

    public void SetEquiped(bool is_equip)
    {
        _recordItem.is_equip = is_equip;
        if (obEquiped != null)
            obEquiped.SetActive(is_equip);
    }

    public void SetSelected(bool is_selected)
    {
        if (obSelected != null)
            obSelected.SetActive(is_selected);
    }

    public void SetActiveTickBorder(bool active)
    {
        if (obTickBorder != null)
            obTickBorder.SetActive(active);
        if (active == false && obTick != null)
            SetActiveTick(false);
    }

    public void SetActiveTick(bool active)
    {
        if (obTick != null)
            obTick.SetActive(active);
        if (obChooseAmount != null)
            obChooseAmount.SetActive(active);
    }

    private void Click()
    {
        if (obTickBorder.activeSelf)
        {
            SetActiveTick(!obTick.activeSelf);
        }

        onClick?.Invoke(this);
    }

    private void ClickUse()
    {
        if (isNotConsumable)
        {
            onEquipItem?.Invoke();
            SetEquiped(true);
            RecordItemEquip recordItem = new RecordItemEquip();
            recordItem.itemID = _recordItem.id;
            recordItem.itemType = _inventoryItemType;
            recordItem.is_equip = true;
            Observer.Instance.Notify(ObserverKey.EquipItem, recordItem);
            SetActiveUseWhenSelected(false);
            UserDatas.SetEquipItem(recordItem);
        }
        else
        {

        }
    }

    private void ClickUnuse()
    {
        SetEquiped(false);
        RecordItemEquip recordItem = new RecordItemEquip();
        recordItem.itemID = _recordItem.id;
        recordItem.itemType = _inventoryItemType;
        recordItem.is_equip = false;
        UserDatas.SetEquipItem(recordItem);
        Observer.Instance.Notify(ObserverKey.EquipItem, recordItem);
        SetActiveUseWhenSelected(true);

    }

    private void ClickBuy()
    {

    }

    private void ClickMinus()
    {
        int number_delete = recordItem.number_delete;
        number_delete--;
        if (number_delete < 1)
            number_delete = 1;
        _recordItem.number_delete = number_delete;
        SetAmountDelete();
        onMinusPlus?.Invoke(recordItem);
    }

    private void ClickPlus()
    {
        int number_delete = recordItem.number_delete;
        int amount = recordItem.amount;
        number_delete++;
        if (number_delete > amount)
            number_delete = amount;
        _recordItem.number_delete = number_delete;
        SetAmountDelete();
        onMinusPlus?.Invoke(recordItem);
    }

    private void SetAmountDelete()
    {
        if (txAmountDelete != null)
            txAmountDelete.text = $"{recordItem.number_delete}/{recordItem.amount}";
    }

    public void SetActiveUseWhenSelected(bool active, bool is_force = false)
    {
        if (obUse != null)
            obUse.SetActive(active);

        if (obUnuse != null)
        {
            obUnuse.SetActive(is_force ? active : isNotConsumable ? !active : false);
        }
    }
}
