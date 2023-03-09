using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[UIPanelPrefabAttr("PopupSelectCharacter", "PopupCanvas")]
public class PopupSelectCharacter : BasePanel
{
    private RecordCharacter[] _recordCharacters = null;
    private RecordCharacter[] recordCharacters
    {
        get
        {
            if (_recordCharacters == null)
                _recordCharacters = DataController.Instance.characterVO.GetDatasByName<RecordCharacter>("CharactersInfo");
            return _recordCharacters;
        }
    }

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
    private ItemSelectCharacter currentItemSelected = null;
    private List<ItemSelectCharacter> lstCurrentItems = new List<ItemSelectCharacter>();
    [SerializeField]
    private Button btClose, btSelect;
    private Dictionary<int, RecordCharacter> userCharacters = new Dictionary<int, RecordCharacter>();
    private bool isCreatedCharacter = false;
    public UnityAction onClose = null;

    protected override void Awake()
    {
        base.Awake();
        if (btClose != null)
            btClose.onClick.AddListener(Close);
        if (btSelect != null)
            btSelect.onClick.AddListener(ClickSelect);
    }

    private void OnEnable()
    {
        GetUserCharactes();
        CreateCharacters();
        UpdateItemCharacterState();
    }

    // Start is called before the first frame update
    void Start()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.offsetMin = Vector2.zero;

    }

    private void GetUserCharactes()
    {
        RecordCharacter[] user_characters = UserDatas.user_Data.user_characters;
        if (user_characters == null || user_characters.Length == 0) return;
        int length = user_characters.Length;
        for (int i = 0; i < length; i++)
        {
            RecordCharacter recordCharacter = user_characters[i];
            if (!userCharacters.ContainsKey(recordCharacter.id))
                userCharacters.Add(recordCharacter.id, recordCharacter);
        }
    }

    private void CreateCharacters()
    {
        if (recordCharacters == null || isCreatedCharacter) return;
        int length = recordCharacters.Length;
        for (int i = 0; i < length; i++)
        {
            RecordCharacter record = recordCharacters[i];
            ItemPool item_manager = itemPool.GetInstance();
            item_manager.transform.SetParent(parentItem);
            item_manager.transform.localPosition = Vector3.zero;
            item_manager.transform.localScale = Vector3.one;
            item_manager.gameObject.SetActive(true);
            ItemSelectCharacter itemSelectCharacter = item_manager.GetComponent<ItemSelectCharacter>();
            itemSelectCharacter.Init(record, ClickItem);
            lstCurrentItems.Add(itemSelectCharacter);
        }
        isCreatedCharacter = true;
    }

    private void UpdateItemCharacterState()
    {
        if (lstCurrentItems == null || lstCurrentItems.Count == 0) return;
        currentItemSelected = null;
        int length = lstCurrentItems.Count;
        for (int i = 0; i < length; i++)
        {
            ItemSelectCharacter itemSelectCharacter = lstCurrentItems[i];
            int id_character = itemSelectCharacter.recordCharacter.id;
            itemSelectCharacter.SetSelected(false);
            itemSelectCharacter.SetLock(!userCharacters.ContainsKey(id_character));
            itemSelectCharacter.SetEquiped(UserDatas.user_Data.info.current_selected_character == id_character);
        }
    }

    private void ClickItem(ItemSelectCharacter item)
    {
        if (currentItemSelected != null)
        {
            if (currentItemSelected == item) return;
            currentItemSelected.SetSelected(false);
        }
        currentItemSelected = item;
        currentItemSelected.SetSelected(true);
    }

    private void ClickSelect()
    {
        if (currentItemSelected == null) return;
        RecordCharacter recordCharacter = currentItemSelected.recordCharacter;
        UserDatas.user_Data.info.current_selected_character = recordCharacter.id;

        RecordNetworkCharacter recordNetworkCharacter = new RecordNetworkCharacter();
        recordNetworkCharacter.id = recordCharacter.id;
        recordNetworkCharacter.isOurs = true;

        Observer.Instance.Notify(ObserverKey.ChangeCharacter, recordNetworkCharacter);
        Close();
    }

    private void Close()
    {
        onClose?.Invoke();
    }

}
