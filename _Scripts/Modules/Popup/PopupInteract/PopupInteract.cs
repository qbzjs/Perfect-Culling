using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[UIPanelPrefabAttr("PopupInteract", "PopupCanvasInteract")]
public class PopupInteract : BasePanel
{
    [SerializeField]private Transform parentInteractType;
    [SerializeField] private GameObject prefabsInteractionType;
    private TextPopUpInteract [] itemText;
    [SerializeField] private Sprite rimClick;
    [SerializeField] private Sprite rimUnClick;
    private TextPopUpInteract previousItemText;
    private int currentLight = 0;
    private int typeLength = 1;
    private ResponseInteraction[] responseInteraction;
    public void SetInteractionType(ResponseInteraction[] response)
    {
        this.responseInteraction = response;
        if (response != null)
        {
            typeLength = response.Length;
            itemText = new TextPopUpInteract[typeLength];
            for (int i = 0; i < typeLength; i++)
            {
                if(prefabsInteractionType!=null && parentInteractType != null)
                {
                    TextPopUpInteract item = CreateController.instance.CreateObjectGetComponent<TextPopUpInteract>(prefabsInteractionType, Vector3.zero, parentInteractType);
                    item.SetInteractTypeText(response[i].InteractionType);
                    itemText[i] = item;
                }
            }
            if (rimClick == null || rimUnClick == null) return;
            itemText[0].imageClick.sprite = rimClick;
            previousItemText = itemText[0];
            currentLight = 0;
            Observer.Instance.Notify(ObserverKey.CurrentChooseInteract, responseInteraction[currentLight]);
        }

    }
    private void RemoteFeature()
    {
        Vector2 scrollDelta = Input.mouseScrollDelta;
        if (scrollDelta.y > 0)
        {
            currentLight--;
            if (currentLight < 0) currentLight = 0;
            ChooseFeature(currentLight);
        }
        if (scrollDelta.y < 0)
        {
            currentLight++;
            if (currentLight >= typeLength) currentLight = typeLength-1;
            ChooseFeature(currentLight);
        }
    }
    private void ChooseFeature(int currentLight)
    {
        if (rimClick == null || rimUnClick == null) return;
        if (previousItemText != itemText[currentLight])
        {
            itemText[currentLight].imageClick.sprite = rimClick;
            previousItemText.imageClick.sprite = rimUnClick;
            previousItemText = itemText[currentLight];
            Observer.Instance.Notify(ObserverKey.CurrentChooseInteract, responseInteraction[currentLight]);
        }
    }
    private void Update()
    {
        RemoteFeature();
    }
    private void OnDisable()
    {
        Destroy(gameObject);
    }
}
