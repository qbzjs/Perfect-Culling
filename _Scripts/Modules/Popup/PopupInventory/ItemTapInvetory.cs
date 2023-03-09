using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemTapInvetory : MonoBehaviour
{
    [SerializeField]
    private GameObject obName, obLine, obNotSelected, obSelected;
    [SerializeField]
    private Button btTap;
    public GameObject obParentItem;
    public Transform[] parentTransform ; 
    public UnityAction<ItemTapInvetory> onClick;
    private int lengthArrayRecord = 0;
    private int firstArrayTap = 0;
    private RecordItemInventory[] _recordItemInventories = null;
    public RecordItemInventory[] recordItemInventories
    {
        get
        {
            string[] name_of_all_component = gameObject.name.Split('/');
            if (name_of_all_component.Length == 1)
            {
                obParentItem.SetActive(false);
            
                    InventoryItemType item_type = Ultis.ParseEnum<InventoryItemType>(gameObject.name);
                    _recordItemInventories = UserDatas.GetRecordItemInventoriesByType(item_type).ToArray();
                return _recordItemInventories;
            }
            if (name_of_all_component.Length > 0 && name_of_all_component != null)
            {
                obParentItem.SetActive(true);
                lengthArrayRecord = 0;
                firstArrayTap = 0;
                for (int i = 0; i < name_of_all_component.Length; i++)
                {
                    InventoryItemType item_type = Ultis.ParseEnum<InventoryItemType>(name_of_all_component[i]);
                    List<RecordItemInventory> lst_record = UserDatas.GetRecordItemInventoriesByType(item_type);
                    int lengthLst = lst_record.Count;
                    lengthArrayRecord = lengthArrayRecord + lengthLst;
                }
                if (_recordItemInventories == null)
                {
                    _recordItemInventories = new RecordItemInventory[lengthArrayRecord];
                }
                for (int i = 0; i < name_of_all_component.Length; i++)
                {
                    InventoryItemType item_type = Ultis.ParseEnum<InventoryItemType>(name_of_all_component[i]);
                    List<RecordItemInventory> lst_record = UserDatas.GetRecordItemInventoriesByType(item_type);
                    int lengthLst = lst_record.Count;
                    if (lst_record != null)
                    {
                        for (int k = firstArrayTap; k < lengthLst + firstArrayTap; k++)
                        {
                            _recordItemInventories[k] = lst_record[k- firstArrayTap];
                        }
                        firstArrayTap = firstArrayTap + lengthLst;
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

    private void Click()
    {
        onClick?.Invoke(this);
    }
}