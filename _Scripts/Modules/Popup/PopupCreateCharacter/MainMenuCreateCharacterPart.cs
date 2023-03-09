using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCreateCharacterPart : MonoBehaviour
{
    [SerializeField] private Transform[] parentItem;
    [SerializeField] private GameObject[] ob_Parts;
    [SerializeField] private ItemPartByName[] itemTapInvetories;
    private ItemPartByName currentTapInventory = null;
    private List<ItemPart> lst_CurrentItemInventory = new List<ItemPart>();
    private List<ItemPool> lstCurrentItems = new List<ItemPool>();
    private GameObject ob_CurrentPartObject;

    [SerializeField] private ItemPart part_Prefab;
    private void OnEnable()
    {
        if (itemTapInvetories != null && itemTapInvetories.Length > 0)
        {
            ItemTapClick(itemTapInvetories[0]);
            if (ob_Parts[0] != null)
            {
                ob_Parts[0].SetActive(true);
                ob_CurrentPartObject = ob_Parts[0];
            }
        }
    }

    private void Awake()
    {
        if (itemTapInvetories != null && itemTapInvetories.Length > 0)
        {
            foreach (var item in itemTapInvetories)
            {
                CreateItems(item);
            }
        }
    }

    private void Start()
    {
        if (itemTapInvetories != null && itemTapInvetories.Length > 0)
        {
            int length = itemTapInvetories.Length;
            for (int i = 0; i < length; i++)
            {
                itemTapInvetories[i].number_part = i;
                itemTapInvetories[i].onClick = (tap, number) =>
                 {
                     ItemTapClick(tap);
                     SetAcitivePartContainer(ob_Parts[number]);
                 };
            }
            currentTapInventory = itemTapInvetories[0];
        }
    }

    private void ItemTapClick(ItemPartByName tap)
    {
        if (currentTapInventory != null)
        {
            if (currentTapInventory == tap) return;
            currentTapInventory.SetActiveHighlight(false);
        }
        currentTapInventory = tap;
        currentTapInventory.SetActiveHighlight(true);
    }

    private void SetAcitivePartContainer(GameObject ob)
    {
        if (ob != null)
        {
            if (ob == ob_CurrentPartObject) return;
            ob_CurrentPartObject.SetActive(false);
            ob_CurrentPartObject = ob;
            ob_CurrentPartObject.SetActive(true);
        }
    }
    private void CreateItems(ItemPartByName currentTapInventory1)
    {
        List<RecordItemInventory> _recordItemInventories = currentTapInventory1.recordItemInventories;

        if (_recordItemInventories != null && _recordItemInventories.Count > 0)
        {
            int item_length = _recordItemInventories.Count;
            for (int i = 0; i < item_length; i++)
            {
                RecordItemInventory record_item = _recordItemInventories[i];
                ItemPart ob_item = Instantiate(part_Prefab);
                ob_item.Init(record_item, ClickItemInventory);
                foreach (var item in parentItem)
                {
                    if (ob_item.recordItem.itemType.ToString().Equals(item.gameObject.name))
                    {
                        ob_item.transform.SetParent(item);
                        ob_item.transform.localScale = Vector3.one;
                        ob_item.GetComponent<RectTransform>().localPosition = new Vector3(ob_item.GetComponent<RectTransform>().localPosition.x, ob_item.GetComponent<RectTransform>().localPosition.y, 0);
                        if (record_item.is_hide_item) ob_item.gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    private void ClickItemInventory(ItemPart item)
    {
        foreach (var item1 in lst_CurrentItemInventory)
        {
            if (item1.recordItem.itemType == item.recordItem.itemType)
            {
                item1.SetSelected(false);
            }
        }
        lst_CurrentItemInventory.Add(item);
        item.SetSelected(true);
    }
}
