using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowLightAndPopUp : MonoBehaviour
{
    [SerializeField] ItemToShowLight[] btHaveLights;
    [SerializeField] private GameObject[] popUpOfButtons;
    private ItemToShowLight previousButtonComponent;
    private void OnEnable()
    {
        SetDefault();
    }

    private void Start()
    {
        ActiveButtonHotizontal();
    }
    private void ActiveButtonHotizontal()
    {
        if (btHaveLights == null) return;
        int length = btHaveLights.Length;
        for (int i = 0; i < length; i++)
        {
            int i_copy = i;
            btHaveLights[i_copy].GetComponent<Button>().onClick.AddListener(() => { OnClickAButton(i_copy); });
        }
    }
    private void OnClickAButton(int i_copy)
    {
        DisableAllPopUpButton();
        popUpOfButtons[i_copy].SetActive(true);
        previousButtonComponent.SetStatusButton(false);
        btHaveLights[i_copy].SetStatusButton(true);
        previousButtonComponent = btHaveLights[i_copy];
    }

    public void SetDefault()
    {
        int length = btHaveLights.Length;
        for (int i = 0; i < length; i++)
        {
            btHaveLights[i].SetStatusButton(false);
            popUpOfButtons[i].SetActive(false);

        }
        previousButtonComponent = null;

        if (previousButtonComponent == null)
        {
            btHaveLights[0].SetStatusButton(true);
            popUpOfButtons[0].SetActive(true);
        }
        previousButtonComponent = btHaveLights[0];
    }

    private void DisableAllPopUpButton()
    {
        if (popUpOfButtons == null) return;
        int lenght = popUpOfButtons.Length;
        for (int i = 0; i < lenght; i++)
        {
            popUpOfButtons[i].SetActive(false);
        }
    }


}
