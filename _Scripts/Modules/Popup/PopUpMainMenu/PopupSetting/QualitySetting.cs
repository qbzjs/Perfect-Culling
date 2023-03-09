using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class QualitySetting : MonoBehaviour
{
    [SerializeField] private GameObject ob_Options;

    [SerializeField] private Button bt_DropDown;
    [SerializeField] private Image img_Select;
    [SerializeField] private Image img_DeSelect;

    private bool is_OptionsPanelOpen = false;
    private void SetActiveGameObject(GameObject ob, bool active)
    {
        if (ob != null)
            ob.SetActive(active);
    }
    private void SetActionToButton(Button button, UnityAction action)
    {
        if (button != null)
            button.onClick.AddListener(action);
    }
    // Start is called before the first frame update
    void Start()
    {
        SetActionToButton(bt_DropDown, () =>
        {
            is_OptionsPanelOpen = !is_OptionsPanelOpen;
            SetActiveGameObject(ob_Options, is_OptionsPanelOpen);
            SetActiveGameObject(img_Select.gameObject, is_OptionsPanelOpen);
            SetActiveGameObject(img_DeSelect.gameObject, !is_OptionsPanelOpen);
        });
    }
    private void OnEnable()
    {
        DefaultQuality();
    }
    public void DefaultQuality()
    {
        SetActiveGameObject(ob_Options, false);
        SetActiveGameObject(img_Select.gameObject, false);
        SetActiveGameObject(img_DeSelect.gameObject, true);
    }
}
