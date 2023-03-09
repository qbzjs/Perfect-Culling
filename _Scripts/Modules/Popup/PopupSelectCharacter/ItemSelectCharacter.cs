using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class ItemSelectCharacter : MonoBehaviour
{
    [SerializeField]
    private Image icon;
    [SerializeField]
    private GameObject obSelected, obEquiped, obLock;
    [SerializeField]
    private Button btTap;
    public UnityAction<ItemSelectCharacter> onClick;
    private RecordCharacter _recordCharacter;
    public RecordCharacter recordCharacter => _recordCharacter;
    private bool isLock = true;

    private void Awake()
    {
        if (btTap != null)
            btTap.onClick.AddListener(Click);
    }

    public void Init(RecordCharacter record_character, UnityAction<ItemSelectCharacter> on_click)
    {
        _recordCharacter = record_character;
        SetLock(true);
        SetEquiped(false);
        SetSelected(false);
        LoadIcon(_recordCharacter.icon);
        onClick = on_click;
    }

    private void LoadIcon(string icon_name)
    {
        if (icon != null)
        {
            icon.sprite = TexturesManager.Instance.GetSprites(icon_name);
        }
    }

    private void Click()
    {
        onClick?.Invoke(this);
    }

    public void SetLock(bool is_lock)
    {
        isLock = is_lock;
        if (obLock != null)
            obLock.SetActive(is_lock);
    }

    public void SetEquiped(bool is_equip)
    {
        if (obEquiped != null)
            obEquiped.SetActive(is_equip);
    }

    public void SetSelected(bool is_selected)
    {
        if (obSelected != null)
            obSelected.SetActive(is_selected);
    }
}
