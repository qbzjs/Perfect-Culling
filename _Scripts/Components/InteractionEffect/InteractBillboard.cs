using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractBillboard : MonoBehaviour, IInteractionEffect
{
    private Action onDone = null;
    public void Init(GameObject ob1, ResponseInteraction ob2, Action on_done)
    {
        if (ob1 == null)
        {
            on_done?.Invoke();
            return;
        }
        onDone = on_done;
        CharacterController characterController = ob1.GetComponent<CharacterController>();
        characterController.enabled = false;
        GameConfig.gameBlockInput = true;
        BillboardPopUpManager billboardPopUpManager = PanelManager.Show<BillboardPopUpManager>();

        if (billboardPopUpManager != null)
        {
            Ultis.SetActiveCursor(true);
            billboardPopUpManager.actionClose = () => {
                characterController.enabled = true;
                GameConfig.gameBlockInput = false;
                OnDone();
            };
        }
    }

    public void OnDone()
    {
        Ultis.SetActiveCursor(false);
        onDone?.Invoke();
    }
}
