using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemPartByName : MonoBehaviour
{
    [SerializeField]
    private GameObject obName, obLine, obNotSelected, obSelected;
    [SerializeField]
    private Button btTap;
    public UnityAction<ItemPartByName, int> onClick;
    public int number_part;
    private List<RecordItemInventory> _recordItemInventories = null;
    public List<RecordItemInventory> recordItemInventories
    {
        get
        {
            string[] name_of_all_component = gameObject.name.Split('/');
            if (name_of_all_component.Length == 1)
            {
                if (_recordItemInventories == null)
                {
                    InventoryItemType item_type = Ultis.ParseEnum<InventoryItemType>(gameObject.name);
                    _recordItemInventories = UserDatas.GetRecordItemInventoriesByType(item_type);
                }
                return _recordItemInventories;
            }
            if (name_of_all_component.Length > 0 && name_of_all_component != null)
            {
                for (int i = 0; i < name_of_all_component.Length; i++)
                {
                    InventoryItemType item_type = Ultis.ParseEnum<InventoryItemType>(name_of_all_component[i]);
                    List<RecordItemInventory> lst_record = UserDatas.GetRecordItemInventoriesByType(item_type);
                    if (lst_record != null)
                    {
                        if (_recordItemInventories == null)
                        {
                            _recordItemInventories = new List<RecordItemInventory>();
                        }
                            _recordItemInventories.AddRange(lst_record);
                        
                    }
                }
            }
            return _recordItemInventories;
        }
        set
        {
            _recordItemInventories = value;
        }
    }

    private void Awake()
    {
        if (btTap != null)
            btTap.onClick.AddListener(Click);

    }

    public void SetActiveHighlight(bool active)
    {
        if (obName != null)
            obName.SetActive(active);
        if (obNotSelected != null)
            obNotSelected.SetActive(!active);
        if (obSelected != null)
            obSelected.SetActive(active);
        if (obLine != null)
            obLine.SetActive(active);
    }
    private void Add()
    {

    }
    private void Click()
    {
        onClick?.Invoke(this, number_part);
    }
}
